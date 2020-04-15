# Kali Client Dockerfile


FROM kalilinux/kali-rolling 

# Install core apps

RUN apt-get -y update && apt-get -y upgrade && apt-get -y install openssh-client nano iproute2

# Install SSH private key

RUN mkdir /root/.ssh
COPY id_rsa /root/.ssh/
RUN chmod 600 /root/.ssh/id_rsa

CMD ["bash"]