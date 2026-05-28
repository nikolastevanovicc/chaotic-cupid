# Kolokvijum 2 - Haoticni kupidon

Predmet: Softver nadzorno-upravljackih uredjaja  
Datum: 27. maj 2026.

## Zadatak

Kreirati PubSub aplikaciju koja simulira Haoticnog kupidona.

Servis sadrzi dva interfejsa:

- interfejs za osobe
- interfejs za kupidona

Zadatak implementirati u framework-u po zelji:

- WCF
- ASP.NET Core

## Prijava osobe

Igraci se prijavljuju za trazenje partnera preko metode `InitSinglePerson`.

Osoba preko konzole unosi:

- username
- grad
- godine
- broj telefona

Potrebno je ispisati prikladne poruke ako korisnik:

- ne unese nista
- unese karaktere umesto brojeva
- unese negativne brojeve

## Slanje pisama

Kupidon svakih minut salje po jedno ljubavno pismo svim prijavljenim osobama.

Pravila:

- osoba ne sme dobiti pismo od same sebe
- osoba ne sme primiti drugo pismo dok ne potvrdi preko konzole da je primila prethodno

## Algoritam za izbor osobe

Kupidon salje pisma na osnovu poklapanja osoba i nasumicnog faktora.

Score se racuna ovako:

- ista lokacija: `+30` poena
- slicne godine, odnosno `+-2` godine: `+20` poena
- nasumicni faktor: `+0-100` poena

Za nasumicni faktor koristiti klasu `RNGCryptoServiceProvider`.

Pismo se salje osobi sa najvecim score-om.

## Prikaz primljenog pisma

Kada pismo stigne do osobe, na konzoli se ispisuju detalji osobe od koje je dobila pismo.

Uz detalje se nasumicno dodaje jedna od poruka:

- `Radujem se nasem susretu!`
- `Zelim da se upoznamo.`
- `Nisam zainteresovan/a za upoznavanje.`

Ako je poruka `Nisam zainteresovan/a za upoznavanje.`, broj telefona se ne ispisuje.

## Blokiranje korisnika

Korisnik moze da blokira odredjenog user-a, tako da ne moze da dobije pismo od njega.

Komanda:

```text
/block username
```
