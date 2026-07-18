# Piano di implementazione

Basato su `Project-scope.md` e `tech-stack.md`. Le fasi sono sequenziali per dipendenza tecnica; le attività "Da definire" nello scope sono indicate come blocchi decisionali da chiudere prima della fase corrispondente.

---

## Fase 0 — Setup e decisioni preliminari
Obiettivo: sbloccare le aree ancora aperte nello scope prima di iniziare a scrivere codice su cui poi torneremmo indietro.

- [x] Autenticazione/autorizzazione: ASP.NET Identity + JWT (claim custom `TenantId`, `Ruolo`)
- [x] Hosting: on-premise / server privato
- [x] Provider AI: OpenRouter (via LiteLLM) → modello `openrouter/openai/gpt-oss-120b`, inference provider Cerebras, con Structured Outputs
- [x] Storage allegati: filesystem su server
- [x] Provider email in ingresso: webhook provider (es. SendGrid Inbound Parse / Mailgun)
- [x] Escalation AI → operatore: solo su richiesta esplicita del cliente
- [x] Knowledge base: alimentata automaticamente dallo storico ticket risolti, per-tenant
- [x] Integrazioni con sistemi esistenti: nessuna per ora, architettura API-based non preclusiva
- [x] Registrazione clienti: self-signup con verifica email
- [x] Policy privacy/GDPR (requisiti minimi): retention allegati, consenso in fase di registrazione, diritto di cancellazione account — dettaglio implementativo in Fase 6
- [x] Creare repository Git e struttura solution (.sln) multi-progetto — repo su GitHub (`alessandroPicchiotti/-AI-Helpdesk`), solution `AiHelpdesk` con progetti `AiHelpdesk.Core`, `AiHelpdesk.Infrastructure`, `AiHelpdesk.Api` (Web API a controller), `AiHelpdesk.Client` (Blazor WASM), riferimenti tra progetti impostati secondo Clean Architecture
- [ ] Configurare CI di base (build + test) e ambienti (dev/staging)
- [ ] Configurare file `.env` con `OPENROUTER_API_KEY` e caricamento sicuro della configurazione

> **Nota tecnica**: il pacchetto `Microsoft.OpenApi` 2.0.0 (dipendenza transitiva di `Microsoft.AspNetCore.OpenApi` 10.0.10 nel progetto API) ha una vulnerabilità nota di gravità alta ([GHSA-v5pm-xwqc-g5wc](https://github.com/advisories/GHSA-v5pm-xwqc-g5wc)). La versione corretta è solo nella serie 3.x, non ancora compatibile con il generatore di codice di `Microsoft.AspNetCore.OpenApi` per .NET 10 (nessuna versione più recente disponibile al momento). Da monitorare e aggiornare non appena Microsoft rilascia una versione compatibile.

---

## Fase 1 — Fondamenta architetturali (Core + Infrastructure)
Obiettivo: scheletro Clean Architecture funzionante, senza feature di business ancora complete.

**Progetto Core**
- [x] Creare progetto `Core` (class library, nessuna dipendenza esterna)
- [x] Definire entità di dominio base: `Tenant`, `User` (con ruoli), `Ticket`, `TicketMessage`, `Attachment`
- [x] Definire enum: `TicketAssignmentStatus` (NonAssegnato/Assegnato/Riassegnazione), `TicketWorkStatus` (InLavorazione/InAttesaCliente/Risolto/Chiuso/Riaperto), `TicketPriority` (Normale/Media/Alta), `UserRole` (AdminPiattaforma/AdminTenant/ManageHelpdesk/Addetto/Cliente), `TicketMessageChannel` (Email/Ai/Chat)
- [x] Definire interfacce repository (`ITicketRepository`, `IUserRepository`, `ITenantRepository`)
- [x] Definire interfacce servizi di dominio (`IAssignmentService`, `ISlaCalculator`, `INotificationService`, `IAiClassificationService`)

**Progetto Infrastructure**
- [ ] Creare progetto `Infrastructure`, riferimento a `Core`
- [ ] Configurare `DbContext` EF Core con isolamento multi-tenant (filtro globale per `TenantId`)
- [ ] Creare mapping/configurazioni entità (Fluent API)
- [ ] Prima migrazione EF Core e provisioning database SQL Server
- [ ] Implementare repository concreti
- [ ] Implementare servizio invio email (per notifiche)

**Progetto API**
- [ ] Creare progetto `API` (Controller-based Web API, .NET 10), riferimenti a `Core` e `Infrastructure`
- [ ] Configurare Dependency Injection, `appsettings` per ambiente
- [ ] Configurare autenticazione/autorizzazione (in base a decisione Fase 0)
- [ ] Configurare middleware multi-tenant (risoluzione tenant da claim/token)
- [ ] Configurare Swagger/OpenAPI

**Progetto Client**
- [x] Creare progetto Blazor WASM
- [x] Integrare MudBlazor (ultima versione, 9.7.0), tema base con `MudThemeProvider`/`MudLayout`
- [ ] Configurare `HttpClient` verso API e gestione token di autenticazione
- [ ] Layout applicativo base (navigazione, autenticazione) — struttura scaffold presente, contenuti reali in Fase 2/3

---

## Fase 2 — Gestione utenti, tenant e autenticazione
- [ ] API: endpoint CRUD tenant (solo admin piattaforma)
- [ ] API: endpoint gestione utenti (creazione, ruoli, associazione a tenant)
- [ ] API: login/registrazione (secondo modalità decisa in Fase 0)
- [ ] Client: pagine login/registrazione
- [ ] Client: gestione utenti (admin tenant) — lista, creazione, modifica ruolo
- [ ] Client: gestione tenant (admin piattaforma)
- [ ] Test: isolamento dati tra tenant (nessun cross-tenant leak)

---

## Fase 3 — Ciclo di vita del ticket (core business)
- [ ] API: endpoint creazione ticket (da form cliente, con priorità e assegnazione automatica via email operatore)
- [ ] API: endpoint ricezione ticket via email (webhook da provider tipo SendGrid Inbound Parse / Mailgun)
- [ ] API: endpoint lista ticket con filtering/sorting (per stato, priorità, operatore, tenant)
- [ ] API: endpoint dettaglio ticket con cronologia comunicazioni
- [ ] API: endpoint cambio stato assegnazione (assegna/riassegna) — ruolo Manage Helpdesk/admin
- [ ] API: endpoint cambio stato lavorazione (in lavorazione/in attesa cliente/risolto/chiuso/riaperto)
- [ ] API: endpoint aggiunta messaggio/risposta al ticket con allegato (max 5 MB, validazione tipo/dimensione)
- [ ] Servizio: notifica email su nuovo ticket non assegnato (ad admin + Manage Helpdesk)
- [ ] Servizio: notifiche email/in-app su cambio stato o nuova risposta
- [ ] Servizio: calcolo SLA con sospensione in stato "in attesa cliente"
- [ ] Client: dashboard ticket (lista con filtri, ordinamento, badge priorità/stato)
- [ ] Client: pagina dettaglio ticket (cronologia, upload allegati, cambio stato)
- [ ] Client: form apertura ticket per il cliente (priorità, email operatore opzionale)

---

## Fase 4 — Funzionalità AI
- [ ] Integrazione OpenRouter (via LiteLLM) verso `openrouter/openai/gpt-oss-120b` con Cerebras come inference provider, uso di Structured Outputs per popolare i campi del ticket
- [ ] Implementare `IAiClassificationService` (Core) e relativa implementazione in Infrastructure basata su OpenRouter/LiteLLM
- [ ] Servizio: classificazione automatica ticket per tipologia prodotto
- [ ] Servizio: generazione automatica risposta da knowledge base (per-tenant)
- [ ] Servizio: AI-summary del ticket
- [ ] Servizio: AI-suggested replies per l'operatore
- [ ] Knowledge base per-tenant: pipeline di alimentazione automatica dallo storico ticket risolti (modello dati + processo di indicizzazione/aggiornamento)
- [ ] Client: interfaccia di consultazione knowledge base (per admin tenant/operatori)
- [ ] Client: visualizzazione summary/suggested replies nel dettaglio ticket
- [ ] Client: azione "Parla con un operatore" per attivare l'escalation esplicita alla chat

---

## Fase 5 — Chat cliente-operatore
- [ ] Scelta tecnologia realtime (SignalR, coerente con stack .NET)
- [ ] API: hub SignalR per chat ticket
- [ ] Persistenza messaggi chat nella cronologia comunicazioni del ticket
- [ ] Client: componente chat realtime (cliente e operatore)
- [ ] Notifiche presenza/nuovo messaggio

---

## Fase 6 — Sicurezza, privacy, hardening
- [ ] Implementare policy GDPR definite in Fase 0 (retention, cancellazione dati/allegati, consenso)
- [ ] Audit log su azioni sensibili (assegnazione, cambio stato, accesso dati cliente)
- [ ] Verifica isolamento multi-tenant end-to-end (penetration test leggero / test automatizzati)
- [ ] Rate limiting / protezione endpoint pubblici (form apertura ticket, login)
- [ ] Validazione e scansione allegati (dimensione, tipo file, eventuale antivirus)

---

## Fase 7 — Osservabilità e KPI
- [ ] Strumentazione per KPI: tempo medio prima risposta, % risolti solo AI, rispetto SLA
- [ ] Dashboard reportistica (per admin tenant e admin piattaforma)
- [ ] Logging centralizzato ed error tracking
- [ ] Raccolta CSAT post-risoluzione (survey al cliente su chiusura ticket)

---

## Fase 8 — Test, UAT e go-live
- [ ] Test end-to-end sui flussi principali (apertura ticket → AI → assegnazione → chat → chiusura)
- [ ] Test di carico (scala media: centinaia di ticket/giorno)
- [ ] User Acceptance Testing con utenti reali (admin, operatori, clienti pilota)
- [ ] Migrazione dati da sistemi esistenti, se prevista (dipende da decisione Fase 0)
- [ ] Piano di rollout (pilota su un tenant, poi estensione)
- [ ] Go-live e monitoraggio post-lancio
