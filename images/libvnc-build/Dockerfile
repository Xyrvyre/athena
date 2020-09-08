# Dockerfile used to build updated libvncserver library for x11vnc

FROM ubuntu

LABEL version="1.01"
LABEL description="Image used to build libvncserver with fixed websocket support for Ubuntu-based images"

ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update &&\
	apt-get -y install --no-install-suggests --no-install-recommends \
	cmake \
	make \
	git \
	libsdl2-dev \
	liblzo2-dev \
	gnutls-dev \
	libgcrypt-dev \
	wine-stable \
	ca-certificates \
	gcc+ &&\
	git clone https://github.com/LibVNC/libvncserver.git &&\
	mkdir -p libvncserver/build
	
WORKDIR /libvncserver/build

RUN cmake .. &&\
	cmake --build . &&\
	chmod 666 ./*
	










