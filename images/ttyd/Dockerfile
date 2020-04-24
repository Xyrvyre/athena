# Kali SSH Server Dockerfile (for TTY)
# "(.*\.){0,1}(Athena\.test-net).*"
FROM kali-ssh-client 

RUN useradd test
RUN echo "test:test" | chpasswd
RUN echo "root:toor" | chpasswd
COPY ttyd /usr/bin
# RUN echo '/usr/sbin/sshd -D &' > /root/run.sh
ENTRYPOINT ["/usr/bin/ttyd"]
# RUN chmod +x /root/run.sh
EXPOSE 22
EXPOSE 8080
CMD ["-p 8080", "login"]