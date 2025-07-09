# Mobile data drone control

## 🎯 Goal
The objective of this project is to create affordable, easy-to-build RC airplanes/drones ect. Low latency 3G.

## 🛠️ Parts List
- **Raspberry Pi Zero 2 W**  
  The main controller of the airplane, running light server distribution CLI for processing and control tasks. [Raspberrypi disk image download link](https://www.dropbox.com/scl/fi/6emmwnl3nn47dhc098f74/RPI2W_3Gcontrol.rar?rlkey=m8sl33uylj7zco0t74orxoeaf&e=1&st=ldmvazoj&dl=0). But it should work in any .net enviorment.
- **3G Modem**  
  Connected via USB to the Raspberry Pi Zero 2 W. Provides internet connection.
- **USB camera**  
  lightweight low power USB camera. Make sure it doesn't require too much setting up to use with Linux. To save yourself time. Opted out of zerocam's camera because the cable was too weak.
- **Servo controller**  
  To control the ailerons, elevators ect.
  I use pca9685.
- **Servos, Motors, Battery**  
  Essential components for controlling flight surfaces, propulsion, and powering the system.
- **Tailscale**
  Used to avoid hassle with NAT.         Automatically sets up the best connection    for your drone based on NAT structure.

## ✅ Current Progress / Todo
- Everything works.
- Would be nice to have GPS, Battery voltage, video recording.
- Soon I'll add blueprints on how to solder everything, and instructions on how to use.
- Implement a configuration file.

## 🤝 Contributing
Contributions are welcome! Please submit issues or pull requests for any suggestions or improvements.

### 📄 License
This project is licensed for non-commercial use.  
For commercial licensing, contact: drone-project.diner481@slmail.me

---
*Happy flying and happy coding!*
