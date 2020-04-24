# Kali SSH Server Dockerfile

FROM kalilinux/kali-rolling 

# Install core apps

RUN apt-get -y update && apt-get -y upgrade && apt-get -y install openssh-server iproute2 iputils-ping

# Configure SSH server

RUN mkdir /var/run/sshd
RUN mkdir /root/.ssh
COPY authorized_keys /root/.ssh/
COPY ttyd /usr/bin

RUN echo 'PermitRootLogin yes' > /etc/ssh/sshd_config
RUN useradd test
RUN echo "test:test" | chpasswd
RUN echo "root:toor" | chpasswd
RUN echo "/usr/sbin/sshd -D &" >> /root/run.sh
RUN echo "/usr/bin/ttyd -p 80 login" >> /root/run.sh
RUN chmod +x /root/run.sh
ENTRYPOINT ["/bin/sh"]
CMD ["/root/run.sh"]

EXPOSE 22
EXPOSE 80


