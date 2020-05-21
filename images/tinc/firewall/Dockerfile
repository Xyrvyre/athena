FROM alpine

COPY ip_forward /proc/sys/net/ipv4/
RUN apk add tinc
COPY tinc/dmz/ /etc/tinc/dmz

COPY tinc/external/ /etc/tinc/external
COPY tinc/internal/ /etc/tinc/internal
RUN ls /etc/tinc/
CMD sh
