## Problem
- Continuo richieste di assistenza da parte dei clienti, che impiegano troppo tempo a ricevere risposte.
- Poco personale addetto a rispondere alle richieste di assistenza.
- Costi di gestione elevati a causa della necessità di personale aggiuntivo.
- Sistemi informatici obsoleti e scollegati tra loro.  
- Continue email slagate tra di loro, perdita di contesto e continuità delle richiesta     


## solution
- Implementare un sistema di intelligenza artificiale in grado di rispondere automaticamente alle richieste di assistenza.
- Utilizzare un sistema, se richiesto dal cliente, in grado di mettere in comunicazione tramite chat cliente ed operatore.
- Mantenere una cronologia unica e continuativa dello scambio di comunicazioni (email, AI, chat operatore) legata al ticket, con possibilità di allegare file (limite 5 MB per allegato), per i casi in cui la richiesta non venga risolta immediatamente né dall'AI né dalla chat con l'operatore.

## Architettura multi-tenant
- Il sistema deve supportare più organizzazioni/aziende clienti (tenant) sulla stessa piattaforma, con isolamento dei dati tra tenant (ticket, utenti, knowledge base non visibili tra tenant diversi).
- Ruoli previsti: **admin di piattaforma** (gestione tenant), **admin di tenant** (gestione operatori e configurazione del proprio tenant), **Manage Helpdesk** (assegna e riassegna i ticket agli operatori), **addetti assistenza** (operatori del tenant), **clienti registrati** (solo del proprio tenant).
- Knowledge base e classificazione AI per tipologia di prodotto devono essere configurate per singolo tenant.

## Features
- Ricevere email di supporto e creare ticket
- Auto-generare risposte di tipo umano usando una knowledge base (per-tenant)
- Lista di ticket con filtering e sorting
- Visualizzazione del ticket, inclusa la cronologia completa delle comunicazioni (email, AI, chat, allegati)
- AI-powered ticket classification per tipologia di prodotto con relativo sistema di gestione per la tipologia di prodotto identificata
- AI-summaries per classificazione del ticket
- AI-suggested replies per classificazione del ticket
- Gestione utenti (admin piattaforma, admin tenant, Manage Helpdesk, addetti assistenza, clienti solo registrati)
- Dashboard per visualizzare e gestire tutti i ticket
- Chat con cliente per approfondire richieste di assistenza in caso di non possibilità di risoluzione immediata da parte dell'AI
- Cronologia comunicazioni con possibilità di allegare file (max 5 MB) per i casi non risolti immediatamente
- Gestione stati del ticket su due dimensioni indipendenti:
  - **Stato di assegnazione**: non assegnato (stato iniziale alla creazione) → assegnato → riassegnazione (ritorno a non assegnato con storico del passaggio)
  - **Stato di lavorazione**: in lavorazione → in attesa cliente → risolto → chiuso, con possibilità di riapertura (riaperto) che riporta il ticket in lavorazione
- Notifica via email ad admin e ruolo Manage Helpdesk alla ricezione di un nuovo ticket (stato "non assegnato"), per l'assegnazione manuale all'operatore
- Assegnazione automatica del ticket all'operatore se il cliente indica esplicitamente l'email dell'operatore in fase di apertura richiesta
- Gestione priorità del ticket, selezionabile dal cliente in fase di apertura: **alta**, **media**, **normale** — collegata a SLA per tempi di risposta
- Notifiche (email/in-app) al cambio di stato (assegnazione o lavorazione) o alla ricezione di una risposta sul ticket
- Calcolo SLA che tiene conto dello stato "in attesa cliente" (il tempo di risposta atteso si sospende finché il cliente non replica)
- Regole di escalation dei ticket in caso di mancata presa in carico (da definire: timeout, sollecito, riassegnazione automatica)

## Decisioni di Fase 0
- **Escalation**: attivata solo su richiesta esplicita del cliente, tramite azione dedicata nella UI ("Parla con un operatore").
- **Knowledge base**: alimentata automaticamente dallo storico dei ticket risolti, per-tenant.
- **Integrazioni con sistemi esistenti**: nessuna integrazione prevista in questa fase; l'architettura basata su API non la preclude in futuro.
- **Registrazione clienti**: self-signup con verifica email.
- **Vincoli tecnici**: stack tecnologico, hosting, provider email in ingresso e provider AI definiti in `tech-stack.md`.
- **Privacy e sicurezza (GDPR)** — requisiti minimi: retention policy sugli allegati, consenso al trattamento dati raccolto in fase di registrazione, diritto alla cancellazione dell'account. Il dettaglio implementativo (audit log, data residency, ecc.) è demandato alla fase di hardening del piano di implementazione.

## Da definire / aree aperte
- Nessun punto aperto al momento (tutte le voci della Fase 0 sono state chiuse).

## Metriche di successo (KPI)
- Tempo medio di prima risposta al cliente.
- Percentuale di ticket risolti senza intervento umano (solo AI).
- Customer satisfaction (CSAT) post-risoluzione.
- Riduzione dei costi/personale rispetto alla situazione attuale.
- Rispetto degli SLA per priorità di ticket.