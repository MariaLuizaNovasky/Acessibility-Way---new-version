# Accessibility Way

Site do projeto **Accessibility Way**: um cartão para pedestres com dificuldade de locomoção abrirem o semáforo mais rápido e por mais tempo. O site apresenta o projeto, a equipe, a simulação e o **cadastro de pedestres** para solicitar o cartão.

- **Stack:** ASP.NET Core MVC (.NET 10)
- **Banco:** PostgreSQL no [Supabase](https://supabase.com), com a foto e o documento médico guardados num bucket privado do Supabase Storage
- **Hospedagem:** [Render](https://render.com) (Docker)

> **Por que Render e não Vercel?** A Vercel não executa aplicações ASP.NET Core (ela roda Node.js, Python, Go e Ruby em funções serverless). O Render roda o site como contêiner Docker, sem mudar a linguagem do projeto.

## Estrutura

```
Dockerfile                       imagem usada pelo Render
render.yaml                      blueprint do Render (cria o serviço com um clique)
.env.example                     variáveis de ambiente necessárias
MySQL/Semaforo.mwb               modelo original do banco (MySQL Workbench)
Visual Studio/MeuSiteEmMVC/MeuSiteEmMVC/
  Program.cs                     configuração (porta, banco, storage, /health)
  Controllers/HomeController.cs  páginas e o POST do cadastro
  Data/schema.sql                tabelas, RLS e bucket no Supabase
  Data/AppDbContext.cs           mapeamento Entity Framework
  Models/                        Pedestre e validações do formulário (CPF, CEP, arquivos)
  Services/                      conversão da DATABASE_URL e upload no Supabase Storage
```

## 1. Preparar o Supabase

1. Crie um projeto em [supabase.com/dashboard](https://supabase.com/dashboard) (região sugerida: **South America (São Paulo)**). Guarde a senha do banco.
2. **Tabelas:** o site cria tudo sozinho na primeira vez que inicia (tabelas `pedestre` e `cartao`, RLS e o bucket `documentos-pedestres`). Se preferir fazer à mão, cole o conteúdo de `Data/schema.sql` em **SQL Editor > New query > Run** e defina `DATABASE_AUTO_MIGRATE=false`.
3. Anote os valores:

| Variável | Onde encontrar no Supabase |
|---|---|
| `DATABASE_URL` | Botão **Connect** > **Session pooler** > URI. Troque `[YOUR-PASSWORD]` pela senha do banco. |
| `SUPABASE_URL` | **Project Settings > API** > Project URL (`https://xxxx.supabase.co`) |
| `SUPABASE_SERVICE_ROLE_KEY` | **Project Settings > API Keys** > chave `service_role` (ou uma *secret key* `sb_secret_...`) |

> Use o **Session pooler** (porta 5432 em `pooler.supabase.com`). A conexão direta (`db.xxxx.supabase.co`) só funciona em IPv6, e o Render não tem IPv6 de saída.

> A chave `service_role` dá acesso total ao projeto: ela fica só nas variáveis do Render, nunca no código nem no navegador.

## 2. Publicar no Render

1. Em [dashboard.render.com](https://dashboard.render.com) clique em **New > Blueprint** e conecte este repositório do GitHub.
2. O Render lê o `render.yaml` e pede `DATABASE_URL`, `SUPABASE_URL` e `SUPABASE_SERVICE_ROLE_KEY`. Cole os valores do passo anterior.
3. Clique em **Apply**. O primeiro build leva alguns minutos; depois o site fica em `https://accessibility-way.onrender.com` (ou o nome que o Render der).
4. Teste `https://SEU-SITE.onrender.com/health`: deve responder `{"status":"ok"}`.

Sem blueprint: **New > Web Service** > este repositório > Language **Docker**, Health Check Path `/health`, e cadastre as mesmas variáveis em **Environment**.

> No plano gratuito o Render "dorme" o site após 15 minutos sem acesso; a primeira visita depois disso demora cerca de 1 minuto.

A cada `git push` na branch `main` o Render publica a nova versão automaticamente.

## 3. Rodar no computador

Requer o [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```powershell
cd "Visual Studio/MeuSiteEmMVC/MeuSiteEmMVC"
dotnet user-secrets init
dotnet user-secrets set DATABASE_URL "postgresql://postgres.SEU_REF:SENHA@aws-0-sa-east-1.pooler.supabase.com:5432/postgres"
dotnet user-secrets set SUPABASE_URL "https://SEU_REF.supabase.co"
dotnet user-secrets set SUPABASE_SERVICE_ROLE_KEY "sua_chave"
dotnet run
```

Sem essas variáveis o site abre normalmente, mas o cadastro avisa que está indisponível.

Com Docker:

```powershell
docker build -t accessibility-way .
docker run --rm -p 8080:8080 --env-file .env accessibility-way
```

## Dados e LGPD

O cadastro guarda dados pessoais e de saúde. Por isso:

- as tabelas têm **Row Level Security** ligado e nenhuma política, então a API pública do Supabase não devolve nada; só o servidor do site, pela connection string, lê e grava;
- a foto e o documento médico vão para um **bucket privado**, em pastas com nome aleatório (sem CPF ou nome no caminho);
- o formulário valida CPF, CEP, tamanho (até 5 MB) e tipo dos arquivos no servidor.

Para consultar os cadastros, use o **Table Editor** do Supabase com a sua conta.

## Equipe

Christian Alves dos Santos, Gabriele Bublitz, Guilherme Prochnow Meyer, Maria Luiza Novasky e Nicolle Formigari Poncio.
