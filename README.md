# Google Messages Spammer
## This works on un-rooted phones running Google Messages
Run the program on your PC and it will prepare a Messages client in the background, and open the traditional pairing QR code.
![enter image description here](https://github.com/Hana2736/GoogleMessagesSpammer/blob/master/readme_img/sending.gif?raw=true)
```
$ ./GoogleMessagesSpammer.exe

Setting up. This can take a while

XXXXXXXXXXXXX  geckodriver     INFO    Listening on 127.0.0.1:XXXX
Read port: XXXXX
Read port: XXXXX

Messages for Web found
Login to GMessages with the QR in your browser....
```
![Web Browser QR Code Display](https://github.com/Hana2736/GoogleMessagesSpammer/blob/master/readme_img/pairQR.png?raw=true)

Once you authenticate with the QR, you can change my code to do whatever you'd like. You can also check `isRcs` on any loaded conversation to see if it is a live number. In my example project, you can pick someone to send a lot of messages to.

```
...........
Logged in.
Enter phone number to spam: 778-330-2389
Loaded conversation
Conversation is SMS
How many times to send? (or -1)
-1
Write a message, end with !@! to finish.
Haiiiiiii
!@!
Sending....
Sent.
Sending....
Sent.
Sending....
Sent.
Sending....
...........
```
