FROM xyrvyre/athena:kali-gui-base

RUN DEBIAN_FRONTEND=noninteractive &&\
	apt-get -y update &&\
	apt-get -y install \
	iproute2 \
	iputils-ping \
	firefox-esr \
	nmap \
	wireshark \
	scapy \
	netcat \
	amap \
	tcpdump \
	mariadb-server &&\	
	apt-get -y autoremove &&\
	apt-get clean &&\
	rm -rf /var/lib/apt/lists/* &&\
	unset DEBIAN_FRONTEND 



