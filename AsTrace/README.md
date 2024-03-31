# AsTrace
AsTrace - программа трассировки автономных систем написанная на языыке C#.

![example](https://github.com/egich1502/InternetProtocols2024/assets/43640874/696b2e89-96d9-44b4-8a53-f87dfdc2d3e6)
# Build
Если у вас имеется C# компилятор, вы можете собрать это решение: 
```
git clone https://github.com/egich1502/InternetProtocols2024.git
cd AsTrace
dotnet build
```
Потом вы можете запустить программу, на пример, трассировать AS до e1.ru:
```
\bin\Debug\net7.0\AsTrace.exe e1.ru
```
# Usage
```
AsTrace.exe e1.ru -h 30 -w 2000
```
Запуск программы с параметрами выше, запустит трассировку до хоста e1.ru с максимальным количеством прыжков при поиске узла 30, и таймаутом каждого ответа в 2 секунды.
# Options
|Comand|Short|Description|
|-|-|-|
|--hops|-h|Максимальное число прыжков при поиске узла|
|--wait|-w|Таймаут каждого ответа в миллисекундах|
|--help||Экран помощи|
