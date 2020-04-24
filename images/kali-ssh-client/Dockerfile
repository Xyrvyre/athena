# Kali Client Dockerfile

FROM kalilinux/kali-rolling
RUN apt-get -y update && apt-get -y upgrade && apt-get -y install openssh-client iproute2 iputils-ping

# Configure SSH client

RUN mkdir /var/run/sshd
RUN mkdir /root/.ssh
RUN echo 'StrictHostKeyChecking=accept-new' > /root/.ssh/config

# Install SSH private key

COPY id_rsa /root/.ssh/
RUN chmod 600 /root/.ssh/id_rsa

RUN useradd test
RUN echo "test:test" | chpasswd
RUN echo "root:toor" | chpasswd
COPY ttyd /usr/bin

EXPOSE 22
EXPOSE 80

ENTRYPOINT ["/usr/bin/ttyd"]
CMD ["-p 80", "login"]

