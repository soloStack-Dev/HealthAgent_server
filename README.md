<div align="center">
  <br/>
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://img.shields.io/badge/%20-Medicine-6366f1?style=for-the-badge&logo=heart&logoColor=white">
    <img src="https://img.shields.io/badge/%20-Medicine-6366f1?style=for-the-badge&logo=heart&logoColor=white" height="36">
  </picture>

  <h1 align="center">HealthAgent API</h1>

  <p align="center">
    <strong>AI-Powered Health Symptom Analysis Engine</strong>
    <br/>
    Intelligent health recommendations powered by Groq (llama-3.3-70b) with medical-grade safety guardrails
  </p>

  <p align="center">
    <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 8.0"/>
    <img src="https://img.shields.io/badge/Groq-llama_3.3_70b-38A169?style=flat-square&logo=groq&logoColor=white" alt="Groq"/>
    <img src="https://img.shields.io/badge/SQLite-EF_Core-003B57?style=flat-square&logo=sqlite&logoColor=white" alt="SQLite"/>
    <img src="https://img.shields.io/badge/Swagger-UI-85EA2D?style=flat-square&logo=swagger&logoColor=black" alt="Swagger"/>
  </p>

  <br/>
</div>

---

## Overview

**HealthAgent API** is the backend engine for the HealthRec platform. It uses **Groq** (llama-3.3-70b-versatile) to analyze user-described symptoms, identify possible conditions, recommend actions, and surface trusted medical resources — all within strict safety guardrails.

```
┌──────────────┐     ┌──────────────┐     ┌──────────────────┐
│  React UI    │ ──▶ │  .NET 8 API  │ ──▶ │  Groq API        │
│  (Client)    │ ◀── │  (Server)    │ ◀── │  (Cloud LLM)     │
└──────────────┘     └──────────────┘     └──────────────────┘
```

---

## Architecture

```
HealthAgent.Api/
├── Agents/                  # AI orchestration layer
│   ├── MediAgentKernel.cs   # Core agent: builds prompts, calls Groq, parses output
│   ├── Plugins/             # AI plugins (safety, symptom analysis, web search)
│   ├── Prompts/             # 15 specialized prompt templates
│   └── Skills/              # Skill definitions (MD files used in system prompts)
├── Controllers/             # API controllers
├── Data/                    # EF Core DbContext + configurations
├── Endpoints/               # Minimal API endpoint definitions
├── Entities/                # Domain models (User, Conversation, Message, MedicalResource)
├── Infrastructure/          # Groq/OpenAI-compatible HTTP client, web search service
├── Middleware/               # Exception handling + request logging
├── Models/                  # Request/Response DTOs
├── Services/                # Business logic (conversations, health agent)
└── Common/                  # Shared utilities (ApiResponse, Result<T>, Constants)
```

---

## API Endpoints

### Health Agent (`/api/health`)
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/health/analyze` | Submit symptoms for analysis |
| `POST` | `/api/health/chat` | Conversational symptom analysis |
| `GET` | `/api/health/suggestions` | Get symptom search suggestions |

### Conversations (`/api/conversations`)
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/conversations/` | List all user conversations |
| `GET` | `/api/conversations/{id}` | Get conversation details |
| `DELETE` | `/api/conversations/{id}` | Delete a conversation |
| `POST` | `/api/conversations/{id}/title` | Auto-generate conversation title |

### Resources (`/api/resources`)
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/resources/search` | Search medical resources |
| `GET` | `/api/resources/trending` | Get trending health topics |

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Runtime** | .NET 8.0 (ASP.NET Core Minimal API) |
| **Database** | SQLite / SQL Server via Entity Framework Core 8 |
| **Authentication** | Clerk (frontend-managed, no backend tokens) |
| **AI Model** | llama-3.3-70b-versatile via Groq API |
| **API Docs** | Swagger / Swashbuckle |
| **CORS** | Configured for frontend dev servers |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Groq API key](https://console.groq.com) (free tier available)

### Configuration

1. Update `appsettings.json` with your configuration:
   ```json
   {
     "ConnectionStrings": {
       "SqliteConnection": "Data Source=HealthAgent.db"
     },
     "DatabaseProvider": "Sqlite",
     "Groq": {
       "ApiKey": "",
       "Model": "llama-3.3-70b-versatile",
       "BaseUrl": "https://api.groq.com/openai/v1",
       "TimeoutSeconds": 120,
       "MaxTokens": 2048,
       "Temperature": 0.3
     }
   }
   ```

2. Set your Groq API key via environment variable:
   ```bash
   set GROQ_API_KEY=gsk_your_key_here
   ```

### Run

```bash
dotnet run

# The API starts at http://localhost:5115
# Swagger UI: http://localhost:5115/swagger
```

---

## Docker

A production-ready `Dockerfile` is located at `HealthAgent.Api/Dockerfile`. It uses a multi-stage build:

1. **Build stage** — restores dependencies and publishes a release build using `mcr.microsoft.com/dotnet/sdk:8.0`
2. **Runtime stage** — runs the published output on `mcr.microsoft.com/dotnet/aspnet:8.0`

### Build & Run

```bash
docker build -t health-agent-api -f HealthAgent.Api/Dockerfile .

docker run -d -p 8080:8080 \
  -e DatabaseProvider=Sqlite \
  -e ConnectionStrings__SqliteConnection="Data Source=HealthAgent.db" \
  -e GROQ_API_KEY=gsk_your_key_here \
  health-agent-api
```

> **Note:** The Dockerfile sets `DatabaseProvider=Sqlite` and `ConnectionStrings__SqliteConnection` by default. Set `GROQ_API_KEY` to your Groq API key.

---

## Safety & Ethics

This system implements strict safety guardrails:

- **No medical diagnosis** — The AI provides informational analysis only, never a definitive diagnosis
- **Emergency detection** — High-severity symptoms are flagged with urgent warnings
- **No medication recommendations** — The system explicitly avoids suggesting specific drugs
- **Verified resources** — Only links to trusted medical sources (WHO, Mayo Clinic, CDC, NHS, etc.)
- **Privacy first** — All health data is encrypted and never shared with third parties

> **Disclaimer:** This system is for informational purposes only. Always consult a qualified healthcare professional for medical decisions.

---

## Project Status

✅ Active Development — Core features complete, actively maintained.

<br/>
<hr/>

<div align="center">
  <sub>Built with care for better health insights.</sub>
</div>
