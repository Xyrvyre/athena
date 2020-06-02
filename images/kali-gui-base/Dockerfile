FROM kalilinux/kali-rolling 

LABEL version="0.1"
LABEL description="Base Kali image with Xfce and VNC preconfigured"

ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update &&\
	apt-get install -y --no-install-recommends x11vnc \
	liblzo2-2 \
	xvfb \
	xfce4 \
	gtk2-engines-pixbuf \
	sudo \
	openssh-server \
	openssh-client \
	supervisor \
	ca-certificates \
	wget &&\
	rm -rf /var/lib/apt/lists/*

RUN useradd --create-home --shell /bin/bash kali-user &&\
	echo "root:toor" | chpasswd &&\
	echo "kali-user:user-kali" | chpasswd &&\
	echo 'PermitRootLogin no' > /etc/ssh/sshd_config &&\
	echo 'kali-user ALL=(ALL) NOPASSWD: ALL' | (EDITOR="tee -a" visudo)

RUN mkdir -p /root/.ssh &&\
	wget https://github.com/tsl0922/ttyd/releases/download/1.6.0/ttyd_linux.x86_64 &&\
	mv ttyd_linux.x86_64 /usr/local/bin/ttyd


COPY --from=libvnc-build /libvncserver/build/libvncserver.so.0.9.13 /usr/lib/x86_64-linux-gnu/
COPY supervisord.conf /etc/supervisord.conf


RUN rm /usr/lib/x86_64-linux-gnu/libvncserver.so.1 && \
	ln -s /usr/lib/x86_64-linux-gnu/libvncserver.so.0.9.13 /usr/lib/x86_64-linux-gnu/libvncserver.so.1

RUN chmod 555 /etc/supervisord.conf &&\
	chmod 554 /usr/local/bin/ttyd &&\
	mkdir /var/run/sshd &&\
	mkdir /home/kali-user/.ssh



CMD ["/usr/bin/supervisord", "-n", "--configuration=/etc/supervisord.conf"]

EXPOSE 5900 8080