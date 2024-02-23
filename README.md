# Szakdolgozat
Unity szakdolgozat

## Rövid leírás

Ez a játék egy "labirintus" rendszert generál, mégpedig olyanokat, amik szobákból, valamit az azokat összekötő folyosókból áll. Ezen a pályán található egy végpont, ami a játékos célja, hogy elérjen. Ehhez meg kell keresnie a pályán elrejtett kisebb feladatokat (pl.: karok, nyomólapok). Ha sikerült minden aktiválni, kinyílik a végső szoba ajtaja, így teljesíthető a játék. Viszont nem ilyen könnyő a játékos dolga. Feladatát különböző ellenfelek nehezítik, amik ellen igen nehezen tud közdeni. Egyetlen esélye a futás.

## Rövid technikai összegző
Ez egy egyszerű játék, amit Unity 2021.3.20f1 verzión valósítottam meg.

Használt algoritmusok:
- Delaunay-háromszögelés
- Minimális feszítőfa
- nem artikulációs pont keresés
- A* algoritmus

Ezen felül használom:
- Unity Nav Mesh Agent
- Kezdetleges Prefab-ok


