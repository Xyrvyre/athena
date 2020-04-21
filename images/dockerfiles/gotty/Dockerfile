# Kali SSH Server Dockerfile (for GoTTY)

FROM kalilinux/kali-rolling 

# Install core apps

RUN apt-get -y update && apt-get -y upgrade && apt-get -y install openssh-server iproute2 iputils-ping

# Configure SSH server

RUN mkdir /var/run/sshd
RUN mkdir /root/.ssh
COPY authorized_keys /root/.ssh/
RUN echo 'PermitRootLogin yes' > /etc/ssh/sshd_config
COPY gotty /usr/bin
COPY run.sh /root/run.sh
EXPOSE 22
EXPOSE 8080
CMD sh /root/run.sh