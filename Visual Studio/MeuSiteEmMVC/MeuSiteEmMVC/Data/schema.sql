-- Accessibility Way - estrutura do banco no Supabase (PostgreSQL)
-- Baseado no modelo MySQL original (MySQL/Semaforo.mwb).
-- O script é idempotente: pode rodar mais de uma vez sem perder dados.
-- O app executa este arquivo ao iniciar (desligue com DATABASE_AUTO_MIGRATE=false)
-- ou você pode colar no SQL Editor do Supabase.

create table if not exists public.pedestre (
    id               bigint generated always as identity primary key,
    nome             varchar(120) not null,
    cpf              char(11)     not null unique,
    data_nascimento  date         not null,
    telefone         varchar(20),
    cep              char(8)      not null,
    uf               char(2)      not null,
    cidade           varchar(80)  not null,
    bairro           varchar(80)  not null,
    rua              varchar(120) not null,
    numero_casa      varchar(10),
    complemento      varchar(60),
    nome_condicao    varchar(120) not null,
    tempo_condicao   varchar(12)  not null check (tempo_condicao in ('permanente', 'temporaria')),
    foto_path        text         not null,
    doc_medico_path  text         not null,
    criado_em        timestamptz  not null default now()
);

-- Cartão que abre o semáforo (tabelas Cartao e Cod_Barras do modelo original).
create table if not exists public.cartao (
    id             bigint generated always as identity primary key,
    pedestre_id    bigint      not null references public.pedestre (id) on delete cascade,
    codigo_barras  varchar(64) unique,
    forma_entrega  varchar(20) check (forma_entrega in ('retirada', 'correios')),
    criado_em      timestamptz not null default now()
);

create index if not exists cartao_pedestre_id_idx on public.cartao (pedestre_id);

-- Dados pessoais e de saúde (LGPD): RLS ligado e sem políticas, então a API pública
-- do Supabase (chave anon/publishable) não lê nada. O site acessa pela connection string.
alter table public.pedestre enable row level security;
alter table public.cartao   enable row level security;

-- Bucket privado para a foto e o documento médico (limite de 5 MB por arquivo).
insert into storage.buckets (id, name, public, file_size_limit, allowed_mime_types)
values ('documentos-pedestres', 'documentos-pedestres', false, 5242880,
        array['image/jpeg', 'image/png', 'image/webp', 'application/pdf'])
on conflict (id) do nothing;
