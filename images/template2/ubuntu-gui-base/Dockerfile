#

FROM ubuntu 

LABEL version="0.1"
LABEL description="Base ubuntu image with Xfce and VNC preconfigured"

ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update &&\
	apt-get install -y --no-install-recommends \
	x11vnc \
	liblzo2-2 \
	xvfb \
	xfce4 \
	gtk2-engines-pixbuf \
	sudo \
	openssh-server \
	openssh-client \
	supervisor &&\
	rm -rf /var/lib/apt/lists/*
	

COPY --from=libvnc-build /libvncserver/build/libvncserver.so.0.9.13 /usr/lib/x86_64-linux-gnu/
COPY gotty /usr/local/bin/
COPY supervisord.conf /etc/supervisord.conf


RUN rm /usr/lib/x86_64-linux-gnu/libvncserver.so.1 && \
	ln -s /usr/lib/x86_64-linux-gnu/libvncserver.so.0.9.13 /usr/lib/x86_64-linux-gnu/libvncserver.so.1

RUN chmod 555 /etc/supervisord.conf &&\
	chmod 554 /usr/local/bin/gotty

RUN useradd --create-home --shell /bin/bash ubuntu-user &&\
	echo "root:toor" | chpasswd &&\
	echo "ubuntu-user:user-ubuntu" | chpasswd &&\
	echo 'PermitRootLogin no' > /etc/ssh/sshd_config &&\
	echo 'ubuntu-user ALL=(ALL) NOPASSWD: ALL' | (EDITOR="tee -a" visudo)
	


CMD ["/usr/bin/supervisord", "-n", "--configuration=/etc/supervisord.conf"]

EXPOSE 5900