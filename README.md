# AgilizaDoc

> Gestão inteligente de documentos empresariais, pessoais e jurídicos, do digital ao arquivo físico, em um só lugar.

![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)
![Licença](https://img.shields.io/badge/licen%C3%A7a-privada-lightgrey)

---

## Sobre o projeto

O **AgilizaDoc** nasceu de um problema comum: documentos importantes espalhados entre e-mails, pastas no computador, gavetas e caixas de arquivo. Quando alguém precisa de um contrato, uma certidão ou um alvará, perde tempo procurando — ou descobre que o documento venceu.

O AgilizaDoc centraliza tudo e responde três perguntas rapidamente:

1. **Onde está?** — versão digital e localização exata do original físico.
2. **Está válido?** — controle de vencimentos com alertas antecipados.
3. **Quem pode ver?** — permissões por usuário e histórico de acessos.

### Para quem é

| Perfil | Exemplos de uso |
|---|---|
| **Pessoa física** | RG, CNH, passaporte, escrituras, contratos de aluguel, apólices, comprovantes e documentos dos filhos |
| **Empresas** | Contrato social, alvarás, certidões negativas, notas fiscais, contratos com clientes e fornecedores, documentos de RH |
| **Escritórios de advocacia / setor jurídico** | Procurações, peças processuais, contratos, prazos e documentos vinculados a processos |

---

## Funcionalidades

### Documentos digitais
- Upload de arquivos (PDF, imagens, Word) com pré-visualização
- Organização por **pastas, categorias e etiquetas**
- **OCR**: busca pelo texto dentro de documentos escaneados
- Controle de **versões** (histórico de alterações de um contrato, por exemplo)
- Busca avançada por nome, tipo, data, pessoa, empresa ou conteúdo

### Arquivo físico
- Cadastro da localização do original: **Local → Armário → Prateleira → Caixa → Pasta**
- **Etiquetas com QR Code** para colar nas caixas e pastas; ao escanear, o sistema mostra o conteúdo
- Controle de **empréstimo/retirada**: quem retirou o documento, quando e previsão de devolução
- Registro de **descarte** conforme a tabela de temporalidade

### Módulo jurídico
- Cadastro de **processos** e vínculo de documentos a cada processo
- Controle de **prazos** com calendário e alertas
- Gestão de **contratos**: partes, vigência, valor, renovação automática e reajuste
- Modelos de documentos (procurações, declarações, contratos-padrão)

### Vencimentos e alertas
- Data de validade em qualquer documento (CNH, certidões, alvarás, AVCB, seguros)
- Alertas por **e-mail e notificação** em 30, 15 e 7 dias antes do vencimento
- Painel com documentos **vencidos, a vencer e em dia**

### Segurança e conformidade (LGPD)
- Perfis de acesso: **Administrador, Editor, Leitor**
- Separação entre **espaço pessoal** e **espaço empresarial**
- **Log de auditoria**: quem visualizou, baixou ou alterou cada documento
- Criptografia dos arquivos armazenados
- Compartilhamento por link com senha e data de expiração

### Painel (Dashboard)
- Total de documentos por categoria
- Próximos vencimentos e prazos jurídicos
- Documentos físicos emprestados e em atraso
- Atividades recentes

---

## Fluxo principal

```
Cadastrar documento ──► Anexar arquivo digital ──► Informar local físico (opcional)
         │                                                   │
         ▼                                                   ▼
 Definir validade/prazo                           Gerar etiqueta QR Code
         │
         ▼
 Receber alertas ──► Renovar / Arquivar / Descartar
```

---

## Stack sugerida

| Camada | Tecnologia |
|---|---|
| Front-end | React + TypeScript + Tailwind CSS |
| Back-end | Node.js (NestJS) ou Python (FastAPI) |
| Banco de dados | PostgreSQL |
| Armazenamento de arquivos | Supabase Storage / Amazon S3 |
| Autenticação | Supabase Auth / JWT |
| OCR | Tesseract |
| Notificações | E-mail (Resend) e notificações push |

---

## Estrutura de dados (resumo)

- **Usuário** — nome, e-mail, perfil
- **Espaço** — pessoal ou empresa
- **Documento** — título, tipo, categoria, etiquetas, data de emissão, validade, arquivo, versão
- **LocalFísico** — local, armário, prateleira, caixa, pasta, QR Code
- **Processo** — número, vara/tribunal, partes, status
- **Prazo** — descrição, data, responsável, processo vinculado
- **Empréstimo** — documento, retirado por, data de saída, data de devolução
- **Auditoria** — usuário, ação, documento, data/hora

---

## Roadmap

- [ ] **v0.1** — Cadastro de usuários e login
- [ ] **v0.2** — Upload, categorias e busca de documentos
- [ ] **v0.3** — Controle de validade e alertas por e-mail
- [ ] **v0.4** — Arquivo físico com etiquetas QR Code
- [ ] **v0.5** — Módulo jurídico (processos, prazos e contratos)
- [ ] **v0.6** — OCR e busca por conteúdo
- [ ] **v1.0** — Auditoria, permissões avançadas e dashboard completo
- [ ] **Futuro** — App mobile para escanear documentos com a câmera e assinatura eletrônica

---

## Como executar

> Em breve — instruções de instalação serão adicionadas conforme o desenvolvimento avançar.

```bash
git clone https://github.com/MariaLuizaNovasky/AgilizaDoc.git
cd AgilizaDoc
```

---

## Autora

Desenvolvido por **Maria Luiza Novasky** — [@MariaLuizaNovasky](https://github.com/MariaLuizaNovasky)
