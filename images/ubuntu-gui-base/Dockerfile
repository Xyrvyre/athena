
FROM ubuntu:20.04

LABEL version="1.0"
LABEL description="Base ubuntu image with LXDE and VNC preconfigured"

ENV DEBIAN_FRONTEND noninteractive


RUN apt-get update 

RUN	apt-get install -y \
	x11vnc \
	liblzo2-2 \
	xvfb \
	lxde \
	gtk2-engines-pixbuf \
	sudo \
	openssh-server \
	openssh-client \
	ca-certificates \
	wget \
	supervisor &&\
	apt-get -y autoremove &&\
	rm -rf /var/lib/apt/lists/*
	

# Copying patched libaries from builder image

COPY --from=libvnc-build /libvncserver/build/libvncserver.so.0.9.13 /usr/lib/x86_64-linux-gnu/
COPY supervisord.conf /etc/supervisord.conf


	
RUN rm /usr/lib/x86_64-linux-gnu/libvncserver.so.1 && \
	ln -s /usr/lib/x86_64-linux-gnu/libvncserver.so.0.9.13 /usr/lib/x86_64-linux-gnu/libvncserver.so.1 &&\
	wget https://github.com/tsl0922/ttyd/releases/download/1.6.0/ttyd_linux.x86_64 &&\
	mv ttyd_linux.x86_64 /usr/local/bin/ttyd &&\
	chmod 555 /etc/supervisord.conf &&\
	chmod 554 /usr/local/bin/ttyd &&\
	chmod 555 /tmp/xfce4.sh

# Adding user account, passwords

RUN useradd --create-home --shell /bin/bash ubuntu-user &&\
	echo "root:toor" | chpasswd &&\
	echo "ubuntu-user:user-ubuntu" | chpasswd &&\
	echo 'PermitRootLogin no' > /etc/ssh/sshd_config &&\
	echo 'ubuntu-user ALL=(ALL) NOPASSWD: ALL' | (EDITOR="tee -a" visudo)	
	
 
CMD ["/usr/bin/supervisord", "-n", "--configuration=/etc/supervisord.conf"]

EXPOSE 5900 8080