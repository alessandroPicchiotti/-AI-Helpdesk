# Tech Stack

## Frontend
- **Blazor WebAssembly (WASM)** — .NET
- **MudBlazor** (ultima versione) come libreria di componenti UI

## Backend
- **ASP.NET Web API** su **.NET 10**, basata su **Controller** (no Minimal API)
- **Entity Framework Core** come ORM
- **SQL Server** come database relazionale
- **ASP.NET Identity + JWT** per autenticazione/autorizzazione, con claim custom `TenantId` e `Ruolo` per l'isolamento multi-tenant

## Architettura
Clean Architecture, multi-progetto, con separazione delle responsabilità:

- **Core** — entità di dominio, interfacce (repository, servizi), logica di business indipendente da framework esterni
- **Infrastructure** — implementazione di EF Core (DbContext, migrazioni), repository, integrazioni con servizi esterni (email, AI, storage allegati)
- **API** — progetto Web API basato su Controller, espone gli endpoint, gestisce autenticazione/autorizzazione, orchestrazione delle richieste verso Core/Infrastructure
- **Client (Blazor WASM)** — frontend, consuma la Web API

### Dipendenze tra progetti
- API → Core, Infrastructure
- Infrastructure → Core
- Core → nessuna dipendenza verso gli altri progetti (cuore del dominio, isolato)
- Client (Blazor WASM) → comunica con API solo via HTTP, nessun riferimento diretto ai progetti backend

## Integrazioni esterne
- **Provider AI**: OpenRouter (via LiteLLM) verso il modello `openrouter/openai/gpt-oss-120b`, con **Cerebras** come inference provider. Utilizzo di **Structured Outputs** per popolare in modo affidabile i campi strutturati del ticket (classificazione, summary, suggested replies). Chiave API in `OPENROUTER_API_KEY`, caricata da file `.env`
- **Storage allegati**: filesystem su server (limite 5 MB per allegato, validazione tipo/dimensione lato API)
- **Email in ingresso**: webhook provider (es. SendGrid Inbound Parse / Mailgun) — richiede configurazione DNS/MX dedicata per il dominio di supporto
- **Hosting**: on-premise / server privato

## Da definire
Nessun punto tecnico aperto al momento.
