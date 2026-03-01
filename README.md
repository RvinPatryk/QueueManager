# QueueManager
QueueManager to aplikacja umożliwiająca dodawanie, monitorowanie i równoległe przetwarzanie zadań w kolejce. Zapewnia obsługę logowania operacji, kontroli statusów, różne typy priorytetów oraz mechanizmy raportowania.

---

## Wymagania funkcjonalne

### 1. Zarządzanie zadaniami

- Dodawanie zadania do kolejki
  Użytkownik może dodać nowe zadanie.
  Zadanie posiada:
  - unikalny identyfikator (GUID),
  - nazwę,
  - opis,
  - priorytet od 1 - 10,
  - datę utworzenia,
  - status,
  - osoba przypisana.
- Usuwanie zadania z kolejki
  Użytkownik może usunąć zadanie przed jego wykonaniem.
  Usunięcie nie jest możliwe, gdy zadanie jest w trakcie realizacji.
- Edycję zadania
  Możliwość zmiany nazwy, opisu lub priorytetu.
- Wyświetlanie listy zadań
- Filtrowanie po:
  - nazwie,
  - statusie,
  - priorytecie,
  - dacie utworzenia,
  - osobie przypisanej.
  - 
### 2. Obsługa kolejki

- Automatyczne przetwarzanie zadań
  System przetwarza zadania w wybranej przez administratora kolejności np. **FIFO**, **LIFO**, **priorytet 1–10**, **SJF (Shortest Job First)**, **LJF (Longest Job First)**, **RR (Round Robin)** oraz **Random/Shuffle**.
- Obsługa wielowątkowości
  System umożliwia równoległe wykonywanie wielu zadań.
  Maksymalna liczba równoległych wątków jest konfigurowalna.
- Zmiana statusu zadania
- Obsługa błędów
  W przypadku wyjątku zadanie otrzymuje status „Błąd”.

### 3. Monitorowanie i raportowanie 

- Podgląd stanu kolejki
  - Liczba zadań:
  - oczekujących, 
  - w trakcie,
  - zakończonych,
  - z błędem.
- Logowanie operacji
  Rejestrowanie:
  - dodania zadania,
  - rozpoczęcia przetwarzania,
  - zakończenia,
  - błędu.
- Eksport raportu
  Możliwość eksportu historii zadań do pliku (np. JSON lub CSV).

---

## Wymagania niefukcjonalne

### 1. Wydajność

  System powinien 

### 2. Niezawodność

  System nie może utracić danych w przypadku błędu pojedynczego zadania. W przypadku awarii aplikacji dane powinny być możliwe do odtworzenia (baza danych). Każde zadanie powinno mieć jednoznaczny status. Zapewni niezawodność działania systemu.

### 3. Bezpieczeństwo

  System powinien walidować dane wejściowe. Identyfikatory zadań mają być generowane automatycznie. Dostęp do operacji administracyjnych ograniczony przez autoryzację.

### 4. Użyteczność

  System powinien posiadać intuicyjny, czytelny interfejs; czytelne komunikaty błędów; przejrzyste logi.
