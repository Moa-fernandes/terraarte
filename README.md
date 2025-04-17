# 🌍 TerraArte – Agregador de Notícias Culturais e Científicas

**TerraArte** é um projeto fullstack que realiza **raspagem de notícias em tempo real** a partir de fontes confiáveis do Brasil e do mundo, com foco em arte, ciência, cultura e sociedade.
A busca é feita por **palavra-chave e intervalo de datas**, diretamente dos feeds RSS de grandes portais, sem depender de APIs pagas.

---

## 🔍 Funcionalidades

- 📰 Raspagem de feeds RSS de fontes como:
  - G1
  - Estadão
  - Exame
  - Extra
  - Agência Brasil
  - BBC
  - Google News

- 🔎 Filtro por **palavra-chave** (com busca profunda, regex e sinônimos)
- 🗓️ Filtro por **data de início e fim**
- 🎯 Resultados com:
  - Título
  - Fonte
  - Link original
  - Data de publicação

---

## 🧩 Tecnologias utilizadas

| Camada      | Tecnologia                 |
|-------------|----------------------------|
| Frontend    | Svelte + TailwindCSS + Vite |
| Backend     | ASP.NET Core 8 (C#)         |
| Dados       | Feeds RSS públicos          |

---

## 🚀 Como executar o projeto

### ▶️ Backend (.NET 8)
```bash
cd backend
dotnet restore
dotnet run
```
Servidor: http://localhost:5000

### 🌐 Frontend (Svelte)
```bash
cd frontend
npm install
npm run dev
```
Interface: http://localhost:5173

---

## 💡 Exemplo de chamada à API

```bash
GET /api/noticias?termo=museu&dataInicio=2025-04-01&dataFim=2025-04-18
```

---

## 📦 Objetivo

Criar uma ferramenta leve, rápida e sem dependências comerciais para **consultar notícias culturais, científicas e sociais** com filtros inteligentes — útil para pesquisadores, jornalistas, estudantes e interessados em cultura.

---

🧑‍💻 Desenvolvido por [Moa Fernandes](https://github.com/Moa-fernandes)
