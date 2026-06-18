# Interactive-Data-Analysis-Platform

1. Opis i cel projektu

Data anlysis platform to aplikacja desktopowa do zarządzania danymi dotyczących rynku nieruchomości pozwala na szybki podgląd statystyk takich jak średnia, liczba rekordów czy cena za metr kwadratowy. Użytkownik ma równięż podgląd do szczegółów danej nieruchomosci a dzięki grafom możliwość obserwacji zmiany ceny na porzestrzeni lat. Warstwa biznesowa w pythonie odpowida za czyszczenie danyh dzięki czemu juz na starcie użytkownik dostaje czyste dane bez brakujących wartości lub outlinerów które mogą zaburzyć statystyki.
Wyniki z kolei są prezentowane przez warstwe wpf która wyświetla i pozwala na filtrowanie wyników.

Cel projektu 

Celem projektu było stworzenie aplikacji która pozwoliła by w szybki sposób zarządzać danymi i je analizwoać bez potrzby pisania recznie kody. 

Cele szczegółowe projektu obejmowały:

umożliwienie importowania danych z plików CSV,
sprawdzanie poprawności i oczyszczanie przesłanych danych,
automatyczne wyznaczanie podstawowych statystyk,
prezentowanie wyników w formie tekstowej i graficznej,
przechowywanie historii wykonanych analiz,
wykonanie pełnych operacji CRUD na zapisanych analizach,
zastosowanie mechanizmu logowania i autoryzacji użytkownika,
wykorzystanie Data Bindingu i Commands w aplikacji WPF,
rozdzielenie warstwy interfejsu użytkownika od logiki przetwarzania danych.

Projekt ma charakter edukacyjny i prezentuje praktyczne zastosowanie technologii WPF, REST API, relacyjnych baz danych oraz analizy danych w języku Python.


2. Wykorzystane technologie
   
Frontend
C# – główny język programowania aplikacji desktopowej.
WPF (Windows Presentation Foundation) – technologia wykorzystana do stworzenia graficznego interfejsu użytkownika.
XAML – język używany do definiowania wyglądu i struktury widoków aplikacji.
.NET – platforma uruchomieniowa aplikacji frontendowej.
MVVM – wzorzec wykorzystywany do częściowego oddzielenia interfejsu od logiki aplikacji.
Data Binding – mechanizm łączący właściwości ViewModelu z kontrolkami interfejsu.
ICommand i AsyncRelayCommand – mechanizmy obsługujące akcje użytkownika, między innymi analizowanie i usuwanie zapisanych danych.
ObservableCollection – kolekcja umożliwiająca automatyczne odświeżanie listy analiz w interfejsie.
HttpClient – komunikacja aplikacji WPF z backendem REST API.
Entity Framework Core – obsługa lokalnej bazy danych z poziomu aplikacji C#.
SQLite – lokalne przechowywanie wyników analiz.
LiveCharts2 – prezentowanie danych na wykresach.
MahApps.Metro IconPacks – ikony używane w interfejsie aplikacji.

Backend — REST API
Python – język programowania używany do implementacji backendu i analizy danych.
FastAPI – framework służący do budowy REST API.
Uvicorn – serwer ASGI uruchamiający aplikację FastAPI.
Pandas – biblioteka wykorzystywana do wczytywania, oczyszczania i analizowania danych z plików CSV.
SQLite – baza danych przechowująca użytkowników, historię analiz oraz dane wymagane przez backend.
JWT – mechanizm tokenów wykorzystywany do autoryzacji użytkowników.
JSON – format wymiany danych pomiędzy frontendem i backendem.

Nrazędzie programistyczne 
Pycharm Professonal 
Visual studio insiders 
Git 
GitHub 


3. Funkcjonalnosci

Rejestracja i Logowanie 
Wgranie plików csv 
Anliza danych 
wizualizacja danych 
Historia analiz 
Wyszukiwanie filtrowanie po rekordach 
Lokalny zapis wynikow 


4. Architektura frontend-backend

projekt został zbudowany w architekturze klient-serwer i składa sie dwoch głownych czesci 

frontend - wpf 
bakend - fastapi 

frontend odpowiada za 
wyswietlenie interfejsu graficznego 
wyswietlanie wynikow analizy 
zapis lokalny 
filtrowanie rekordow 
obsluge przyciskow i komend 
komunikacje z backendem

Przykładowe operacje wykonywane przez frontend:

przesłanie pliku CSV,
pobranie historii analiz,
pobranie szczegółów wybranej analizy,
zmiana nazwy analizy,
usunięcie analizy,
logowanie użytkownika.

Backend

Backend został utworzony w języku Python z wykorzystaniem FastAPI.

Odpowiada za:

obsługę endpointów REST API,
rejestrację i logowanie użytkowników,
autoryzację z wykorzystaniem JWT,
przyjmowanie plików CSV,
przetwarzanie i oczyszczanie danych,
obliczanie statystyk,
zapis historii analiz w bazie danych,
udostępnianie wyników frontendowi.

Do analizy danych backend wykorzystuje bibliotekę Pandas.

Komunikacja między warstwami

Frontend i backend komunikują się przy użyciu protokołu HTTP. Dane są przekazywane najczęściej w formacie JSON.

Przykładowy przepływ danych:

Użytkownik
    ↓
Aplikacja WPF
    ↓
ApiService / HttpClient
    ↓
REST API FastAPI
    ↓
Przetwarzanie danych w Pandas
    ↓
Baza danych SQLite
    ↓
Odpowiedź JSON
    ↓
Dashboard i historia w aplikacji WPF

Przykład przesyłania pliku:

1. Użytkownik wybiera plik CSV.
2. Frontend wysyła plik do endpointu backendu.
3. Backend sprawdza i analizuje dane.
4. Wyniki zostają zapisane w bazie danych.
5. Backend zwraca statystyki w formacie JSON.
6. Frontend zapisuje wybrane dane lokalnie.
7. Dashboard prezentuje wyniki użytkownikowi.

Rozdzielenie aplikacji na frontend i backend ułatwia rozwój projektu, testowanie oraz późniejszą rozbudowę systemu.

5. Baza danych

W projekcie zastosowano bazę danych SQLite. Jest to lekka relacyjna baza danych, która nie wymaga instalowania oddzielnego serwera bazodanowego.

Projekt wykorzystuje dwa obszary zapisu danych:

bazę danych backendu,
lokalną bazę danych aplikacji WPF.
Baza danych backendu

Backend korzysta z pliku:

backend/logs.db

Baza przechowuje dane wykorzystywane przez API, między innymi:

konta użytkowników,
informacje o przesłanych plikach,
historię analiz,
nazwy analiz,
daty przesłania,
ścieżki do przetworzonych plików,
statystyki zapisane w formacie JSON.

Przykładowy rekord historii analizy może zawierać:

identyfikator analizy,
nazwę pliku,
datę utworzenia,
dane statystyczne,
ścieżkę do zapisanego pliku.

Baza backendu jest obsługiwana przez kod Python i zapytania SQL.

Lokalna baza danych frontendu

Aplikacja WPF korzysta również z lokalnej bazy SQLite obsługiwanej za pomocą Entity Framework Core.

Baza jest tworzona w katalogu użytkownika:

%LocalAppData%\RealEstateDataPlatform\realestate.db

Przechowuje ona lokalne rekordy analiz w tabeli Analyses.

Model AnalysisRecord zawiera między innymi:

lokalny identyfikator rekordu,
identyfikator analizy po stronie backendu,
nazwę analizy,
datę utworzenia,
statystyki zapisane jako JSON.

Dostęp do danych odbywa się za pomocą klas:

AppDbContext
AnalysisRepository
AnalysisRecord

AppDbContext odpowiada za połączenie z bazą, natomiast AnalysisRepository realizuje operacje zapisu, odczytu i usuwania danych.

Powód zastosowania SQLite

SQLite została wybrana ze względu na:

prostą konfigurację,
brak konieczności uruchamiania osobnego serwera,
niewielkie wymagania sprzętowe,
łatwą integrację z Pythonem i Entity Framework Core,
wystarczającą wydajność dla aplikacji edukacyjnej i desktopowej.

Pliki baz danych zostały dodane do .gitignore, ponieważ zawierają dane generowane lokalnie podczas działania aplikacji.

6. Operacje CRUD

Aplikacja realizuje pełny zestaw operacji CRUD dla zapisanych analiz.

CRUD oznacza:

Create – utworzenie rekordu,
Read – odczyt danych,
Update – aktualizacja danych,
Delete – usunięcie danych.
Create – utworzenie analizy

Nowy rekord jest tworzony po przesłaniu pliku CSV.

Proces obejmuje:

wybór pliku przez użytkownika,
przesłanie pliku do backendu,
analizę i oczyszczenie danych,
zapis informacji o analizie w bazie backendu,
zapis wybranych wyników w lokalnej bazie frontendu.

Przykładowa operacja:

UploadFileAsync()

Po stronie backendu odpowiada jej endpoint obsługujący przesyłanie pliku.

Read – odczyt analiz

Użytkownik może odczytać:

pełną listę wykonanych analiz,
dane wybranej analizy,
zapisane wcześniej statystyki.

Lista analiz jest pobierana przez:

GetLogsAsync()

Szczegóły pojedynczej analizy mogą zostać pobrane na podstawie jej identyfikatora.

W widoku historii rekordy są prezentowane w kontrolce DataGrid.

Update – zmiana nazwy

Użytkownik może zmienić nazwę zapisanej analizy.

Po podaniu nowej nazwy aplikacja:

sprawdza, czy nazwa nie jest pusta,
wysyła żądanie aktualizacji do backendu,
zapisuje nową nazwę,
ponownie pobiera historię analiz,
odświeża tabelę w interfejsie.

Operację wykonuje metoda:

RenameLogAsync()
Delete – usunięcie analizy

Użytkownik może usunąć wybraną analizę.

Przed wykonaniem operacji wyświetlane jest okno potwierdzenia.

Po zaakceptowaniu:

frontend wysyła żądanie usunięcia do backendu,
rekord jest usuwany z bazy backendu,
odpowiadający rekord jest usuwany z lokalnej bazy danych,
kolekcja historii zostaje odświeżona,
rekord znika z tabeli.

Operację wykonuje metoda:

DeleteLogAsync()
Podsumowanie CRUD
Operacja	Funkcjonalność	Przykładowa metoda
Create	przesłanie i zapis nowej analizy	UploadFileAsync()
Read	pobranie historii i szczegółów	GetLogsAsync()
Update	zmiana nazwy analizy	RenameLogAsync()
Delete	usunięcie analizy	DeleteLogAsync()

Zastosowanie pełnego zestawu operacji CRUD umożliwia zarządzanie całym cyklem życia zapisanej analizy.


Instrukcja uruchomienia

Projekt składa się z backendu FastAPI oraz aplikacji frontendowej WPF. Przed uruchomieniem frontendu należy uruchomić backend.

Wymagania

Do uruchomienia projektu potrzebne są:

system Windows,
Python 3.11 lub nowszy,
.NET SDK zgodny z projektem,
Visual Studio z obsługą aplikacji WPF,
biblioteki Python zapisane w pliku requirements.txt,
opcjonalnie PyCharm do uruchamiania backendu.
Pobranie projektu

Repozytorium można sklonować za pomocą polecenia:

git clone ADRES_REPOZYTORIUM

Następnie należy przejść do katalogu projektu:

cd "projekt wpf 2026"
Uruchomienie backendu

Przejdź do folderu backendu:

cd backend

Utwórz środowisko wirtualne:

python -m venv .venv

Aktywuj środowisko:

.\.venv\Scripts\Activate.ps1

Zainstaluj wymagane biblioteki:

pip install -r requirements.txt

Uruchom serwer FastAPI:

python -m uvicorn main:app --reload

Po poprawnym uruchomieniu backend powinien być dostępny pod adresem:

http://127.0.0.1:8000

Dokumentacja endpointów FastAPI jest dostępna pod adresem:

http://127.0.0.1:8000/docs
Uruchomienie frontendu
Otwórz plik rozwiązania lub projekt DataAnalizer w Visual Studio.
Poczekaj na przywrócenie pakietów NuGet.
Ustaw projekt DataAnalizer jako projekt startowy.
Upewnij się, że backend FastAPI jest uruchomiony.
Uruchom aplikację przyciskiem Start lub klawiszem F5.

Frontend można również uruchomić z terminala:

dotnet run --project frontend\DataAnalizer.csproj
Bazy danych

Backend automatycznie korzysta z lokalnej bazy SQLite.

backend/logs.db

Frontend tworzy lokalną bazę w katalogu:

%LocalAppData%\RealEstateDataPlatform\realestate.db

Pliki baz danych nie znajdują się w repozytorium, ponieważ zostały dodane do .gitignore.

Przy pierwszym uruchomieniu odpowiednie pliki i tabele są tworzone automatycznie.

Kolejność uruchamiania
1. Uruchom backend FastAPI.
2. Sprawdź, czy działa adres http://127.0.0.1:8000/docs.
3. Uruchom aplikację WPF.
4. Zarejestruj użytkownika lub zaloguj się.
5. Wybierz plik CSV.
6. Prześlij dane i wyświetl analizę.
7. Otwórz historię analiz.
10. Zrzuty ekranu






