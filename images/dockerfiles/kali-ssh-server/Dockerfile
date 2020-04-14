# Kali SSH Server Dockerfile

FROM kalilinux/kali-rolling 

# Install core apps

RUN apt-get -y update && apt-get -y upgrade && apt-get -y install openssh-server

# Configure SSH server

RUN mkdir /var/run/sshd
RUN mkdir /root/.ssh
COPY authorized_keys /root/.ssh/
RUN echo 'PermitRootLogin yes' > /etc/ssh/sshd_config

EXPOSE 22
CMD ["/usr/sbin/sshd", "-D"]