# Kali SSH Server Dockerfile (for VNC testing)

FROM kalilinux/kali-rolling 

# Install core apps

RUN apt-get -y update && apt-get -y upgrade && apt-get -y install openssh-server iproute2 iputils-ping

# Configure SSH server

RUN mkdir /var/run/sshd
RUN mkdir /root/.ssh
COPY authorized_keys /root/.ssh/
RUN echo 'PermitRootLogin yes' > /etc/ssh/sshd_config
EXPOSE 22

# Configure VNC


RUN apt-get -y install xvfb x11vnc firefox-esr xfce4
ENV DISPLAY :99
RUN echo "rm -f /tmp/*" 
RUN echo "Xvfb :99 &" > /root/run.sh
RUN echo "x11vnc -shared -forever -ncache 6 -display :99 &" >> /root/run.sh
RUN echo "startxfce4" >> /root/run.sh
RUN chmod +x /root/run.sh
EXPOSE 5900
ENTRYPOINT /root/run.sh