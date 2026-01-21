# QueueManager

QueueManager to prosta aplikacja do zarządzania kolejką zadań z możliwością wyboru strategii priorytetyzacji przez użytkownika. Pozwala dodawać, edytować i usuwać zadania oraz ustalać ich kolejność wykonywania w zależności od wybranego trybu, np. **FIFO**, **LIFO**, **priorytet 1–10**, **SJF (Shortest Job First)**, **LJF (Longest Job First)**, **RR (Round Robin)** oraz **Random/Shuffle**.

Projekt sprawdza się w organizacji pracy, planowaniu oraz obsłudze procesów wymagających elastycznego kolejkowania zadań. Może być integrowany z innymi systemami i aplikacjami w celu automatyzacji przepływu pracy.

---

## 📌 Informacje ogólne

- **Nazwa projektu:** QueueManager  
- **Typ aplikacji:** Backend / REST API (ASP.NET Core Web API)  
- **Cel:** zarządzanie kolejką zadań oraz ich przetwarzanie w tle  
- **Autorzy:** Patryk Pisarek, Jakub Kruźlak, Szymon Suchanek  
 
![QueueManager Logo](docs/logo.png)

---

## 📝 Opis programu

QueueManager to aplikacja backendowa służąca do zarządzania kolejkowaniem oraz przetwarzaniem zadań w sposób uporządkowany, kontrolowany i skalowalny. System został zaprojektowany z myślą o scenariuszach, w których wiele zadań musi być realizowanych asynchronicznie, zgodnie z ustalonymi zasadami kolejności oraz priorytetów.

Głównym celem aplikacji jest umożliwienie dodawania, modyfikowania, usuwania oraz monitorowania zadań, które trafiają do kolejki przetwarzania. System wspiera różne strategie kolejkowania, takie jak FIFO (First In, First Out) oraz LIFO (Last In, First Out), a także uwzględnia priorytety zadań, co pozwala na elastyczne sterowanie kolejnością ich realizacji. Dla zadań o tym samym priorytecie zachowana jest deterministyczna kolejność przetwarzania.

Aplikacja automatycznie pobiera zadania z kolejki i realizuje je w tle, obsługując równoległe przetwarzanie oraz zmiany statusów w całym cyklu życia zadania – od momentu utworzenia, przez przetwarzanie, aż po zakończenie lub wystąpienie błędu. W przypadku niepowodzenia możliwe jest ponowne przetworzenie zadania bez wpływu na stabilność całego systemu.

System udostępnia również funkcje monitorowania i podglądu, umożliwiające użytkownikom śledzenie aktualnego stanu kolejki, listy aktywnych i zakończonych zadań oraz historii ich przetwarzania. Dzięki temu aplikacja może pełnić rolę centralnego mechanizmu kolejkowego wykorzystywanego w większych systemach informatycznych, np. do obsługi procesów wsadowych, zadań backgroundowych lub integracji między systemami.

---
