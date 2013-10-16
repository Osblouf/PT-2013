#include <16F877A.h>
#device adc=8
#FUSES NOWDT      //No Watch Dog Timer
#FUSES HS         //Highspeed Osc > 4mhz
#FUSES PUT        //Power Up Timer
#FUSES NOPROTECT  //Code not protected from reading
#FUSES NODEBUG    //No Debug mode for ICD
#FUSES NOBROWNOUT //No brownout reset
#FUSES NOLVP      //No low voltage prgming, B3(PIC16) or B5(PIC18) used for I/O
#FUSES NOCPD      //No EE protection

//utilisation du RS232
#use delay(clock=8000000) //fréquence
#use rs232(baud=57600,parity=N,xmit=PIN_C6,rcv=PIN_C7,bits=8)

//fonction d'envoi RC5
void RC5(char c);

void main(void)
{
  char c;
  
  //boucle
  while(1)
  {
    c = getc();
    RC5(c);
  }
}


//fonction d'envoi RC5
void RC5(char c)
{
  
}
