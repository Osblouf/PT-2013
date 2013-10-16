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

//definition de la pin output
#define PIN_OUTPUT_LED PIN_C2

//fonction d'envoi RC5
void RC5(char c);
void LED_1(void);
void LED_0(void);

void main(void)
{
  //ressources
  char c;
  
  //pin output à 1 (led éteinte)
  output_high(PIN_OUTPUT_LED);
  
  //boucle
  while(1)
  {
    //récupération de l'octet
    c = getc();
    
    //envoi à la LED
    RC5(c);
    
    //repassage en mode repos
    output_high(PIN_OUTPUT_LED);
  }
}


//fonction d'envoi RC5
void RC5(char c)
{
  //deux premiers bits
  LED_1();
  LED_1();
  
  //bits à 0
  LED_0();
  LED_0();
  LED_0();
  LED_0();
  
  //envoi du message :
  if(c & (1 << 7))
    LED_1();
  else
    LED_0();
    
  if(c & (1 << 6))
    LED_1();
  else
    LED_0();
    
  if(c & (1 << 5))
    LED_1();
  else
    LED_0();
  
  if(c & (1 << 4))
    LED_1();
  else
    LED_0();
  
  if(c & (1 << 3))
    LED_1();
  else
    LED_0();
  
  if(c & (1 << 2))
    LED_1();
  else
    LED_0();
  
  if(c & (1 << 1))
    LED_1();
  else
    LED_0();
    
  if(c & (1 << 0))
    LED_1();
  else
    LED_0();
  
}

//fonction bit à 1
void LED_1(void)
{
  output_high(PIN_OUTPUT_LED);
  delay_us(889);
  output_low(PIN_OUTPUT_LED);
  delay_us(889);
}

//fonction bit à 0
void LED_0(void)
{
  output_low(PIN_OUTPUT_LED);
  delay_us(889);
  output_high(PIN_OUTPUT_LED);
  delay_us(889);
}
