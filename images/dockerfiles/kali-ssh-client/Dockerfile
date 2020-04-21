# Kali Client Dockerfile


FROM kali-ssh-server

# Install core apps

RUN apt-get -y update && apt-get -y upgrade && apt-get -y install openssh-client nano iproute2 iputils-ping

# Install SSH private key

COPY id_rsa /root/.ssh/
RUN chmod 600 /root/.ssh/id_rsa

ENTRYPOINT ["/usr/sbin/sshd"]
CMD ["-D"]