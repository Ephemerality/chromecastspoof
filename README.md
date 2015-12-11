# Chromecast Spoof (CSpoof)
A .Net application that sends fake multicast DNS packets, allowing you to cast from a mobile device to a Chromecast on a different network.

Originally designed to play Cardcast (http://www.cardcastgame.com) remotely.

Created by Ephemerality <ephemeral.vilification@gmail.com>

## Requirements
* Mobile device capable of casting to a Chromecast
* Windows computer on the same network as your mobile device
* .Net Framework 3.5 or higher  
* MiscUtil (included)
  
## Instructions
* Forward ports 8008-8009, TCP and UDP, to your Chromecast
* Run the application on a computer on the same network as your mobile device
* Enter the name you want to display (does not need to match your Chromecast)
* Enter the public IP address of your Chromecast
* Click one of the buttons to send once or continuously
* You should now see the fake Chromecast show up on the mobile device
* If the ports are properly forwarded and the correct IP was entered, you should be able to cast to it
* If you have VMWare/VirtualBox installed, you may have to disable the network adapters for it to send through the proper interface...

## Acknowledgements
This product includes software developed by Jon Skeet
and Marc Gravell. Contact skeet@pobox.com, or see 
http://www.pobox.com/~skeet/.