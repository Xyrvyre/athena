FROM nginx

RUN apt-get update
RUN apt-get -y install tinc

COPY files

CMD tincd -n dmz -K4096
