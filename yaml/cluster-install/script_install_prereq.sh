#!/bin/bash
# Installation script for Kubernetes cluster nodes

if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
  exit
fi

# Installing Docker
if ! sudo apt-get update; then
	printf "Update failed.\n\n"
	exit 1
fi

if ! sudo apt-get install -y apt-transport-https ca-certificates curl gnupg-agent software-properties-common; then
	printf "Docker dependency install failed.\n\n"
	exit 1
fi
	
if ! curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -; then
	printf "Failed to add Docker GPG key.\n\n"
	exit 1
fi
if ! sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"; then
	printf "Failed to add Docker repository."
	exit 1
fi
if ! sudo apt-get update; then
	printf "Update failed.\n\n"
	exit 1
fi
if ! sudo apt-get install -y docker-ce docker-ce-cli containerd.io; then
	printf "Docker install failed.\n\n"
	exit 1
fi

printf "\n\e[1mDocker has been installed.\n\e[0m"
# Configuring Docker for Kubernetes
cat << EOF | sudo tee /etc/docker/daemon.json 
{
  "exec-opts": ["native.cgroupdriver=systemd"],
  "log-driver": "json-file",
  "log-opts": {
    "max-size": "100m"
  },
  "storage-driver": "overlay2"
}
EOF
if ! sudo mkdir -p /etc/systemd/system/docker.service.d; then
	printf "Failed to create Docker service directory.\n\n"
	exit 1
fi

if ! sudo systemctl daemon-reload; then
	printf "Failed to reload Docker daemon.\n\n"
	exit 1
fi
if ! sudo systemctl restart docker; then
	printf "Failed to restart Docker daemon.\n\n"
	exit 1
fi
printf "\n\e[1mDocker has been configured to work with Kubernetes.\n\e[0m"
# Installing Kubernetes binaries

if ! sudo apt-get update; then
	printf "Update failed.\n\n"
	exit 1
fi
if ! sudo apt-get install -y apt-transport-https curl; then
	printf "Installation of Kubernetes dependencies failed."
	exit 1
fi
if ! curl -s https://packages.cloud.google.com/apt/doc/apt-key.gpg | sudo apt-key add -; then
	printf "Failed to add Kubernetes GPG key."
	exit 1
fi
cat << EOF | sudo tee /etc/apt/sources.list.d/kubernetes.list
deb https://apt.kubernetes.io/ kubernetes-xenial main
EOF
if ! sudo apt-get update; then
	printf "Update failed.\n\n"
	exit 1
fi
if ! sudo apt-get install -y kubelet kubeadm kubectl; then
	printf "Installation of Kubernetes binaries failed.\n\n"
	exit 1
fi

printf "\n\nInstallation complete.\n\n"

if ! sudo apt-mark hold kubelet kubeadm kubectl; then
	printf "Failed to prevent Kubernetes binaries from being updated.\n\n"
fi
if ! sudo kubeadm config images pull; then
	printf "Failed to pull Kubernetes images. Please check your network connection.\n\n"
fi




















