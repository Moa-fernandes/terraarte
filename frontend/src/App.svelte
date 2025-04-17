<script>
  let termo = '';
  let dataInicio = '';
  let dataFim = '';
  let idiomaSelecionado = '';
  let noticias = [];
  let carregando = false;

  const todasFontes = [
    "G1",
    "Estadão",
    "Exame",
    "Extra",
    "Agência Brasil",
    "BBC",
    "Google News"
  ];
  let fontesSelecionadas = [];

  let modalAberto = false;
  let noticiaSelecionada = null;

  function abrirModal(noticia) {
    noticiaSelecionada = noticia;
    modalAberto = true;
  }

  function fecharModal() {
    modalAberto = false;
    noticiaSelecionada = null;
  }

  function limparFiltros() {
    termo = '';
    dataInicio = '';
    dataFim = '';
    fontesSelecionadas = [];
    idiomaSelecionado = '';
    noticias = [];
  }

  async function buscar() {
    const params = new URLSearchParams();

    if (termo) params.append("termo", termo);
    if (dataInicio) params.append("dataInicio", dataInicio);
    if (dataFim) params.append("dataFim", dataFim);
    if (fontesSelecionadas.length > 0)
      params.append("fontes", fontesSelecionadas.join(","));
    if (idiomaSelecionado)
      params.append("idioma", idiomaSelecionado);

    carregando = true;
    try {
      const res = await fetch(`http://localhost:5000/api/noticias?${params.toString()}`);
      noticias = await res.json();
    } catch (e) {
      alert("Erro ao buscar notícias.");
      noticias = [];
    } finally {
      carregando = false;
    }
  }
</script>

<main class="min-h-screen p-6 max-w-5xl mx-auto text-white">
  <h1 class="text-4xl font-bold text-center mb-10 tracking-tight">📰 TerraArte Notícias</h1>

  <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
    <input type="text" bind:value={termo} placeholder="Palavra-chave"
      class="p-3 rounded-lg bg-slate-800 border border-slate-700 focus:outline-none focus:ring-2 focus:ring-yellow-400" />

    <input type="date" bind:value={dataInicio}
      class="p-3 rounded-lg bg-slate-800 border border-slate-700 focus:outline-none focus:ring-2 focus:ring-yellow-400" />

    <input type="date" bind:value={dataFim}
      class="p-3 rounded-lg bg-slate-800 border border-slate-700 focus:outline-none focus:ring-2 focus:ring-yellow-400" />
  </div>

  <div class="grid md:grid-cols-2 gap-4 mb-6">
    <div class="text-left">
      <h3 class="font-semibold mb-2 text-lg">Fontes:</h3>
      <div class="grid grid-cols-2 md:grid-cols-3 gap-2">
        {#each todasFontes as fonte}
          <label
            class="flex items-center gap-2 p-3 rounded-lg bg-slate-700 hover:bg-slate-600 transition cursor-pointer shadow-inner shadow-black/20">
            <input type="checkbox" bind:group={fontesSelecionadas} value={fonte} class="accent-yellow-500" />
            <span class="text-sm">{fonte}</span>
          </label>
        {/each}
      </div>
    </div>

    <div class="text-left">
      <h3 class="font-semibold mb-2 text-lg">Idioma:</h3>
      <select bind:value={idiomaSelecionado}
        class="p-3 bg-slate-800 border border-slate-700 rounded-lg w-full text-sm focus:outline-none focus:ring-2 focus:ring-yellow-400">
        <option value="">🌐 Todos os idiomas</option>
        <option value="pt">🇧🇷 Português</option>
        <option value="en">🇺🇸 Inglês</option>
      </select>
    </div>
  </div>

  <div class="text-center mb-8 space-y-2">
    <button
      class="bg-yellow-500 hover:bg-yellow-600 text-white font-bold px-6 py-3 rounded-lg shadow-lg transition active:scale-95 disabled:opacity-50"
      on:click={buscar}
      disabled={carregando}>
      {carregando ? 'Buscando...' : '🔍 Buscar Notícias'}
    </button>
    <br />
    <button on:click={limparFiltros} class="text-sm underline text-slate-400 hover:text-white transition">
      Limpar filtros
    </button>
  </div>

  {#if noticias.length > 0}
    <div class="space-y-6">
      {#each noticias as n}
        <div class="bg-slate-800 p-5 rounded-xl shadow-md border border-slate-700">
          <h2 class="text-xl font-bold mb-1">{n.titulo}</h2>
          <p class="text-sm text-gray-400 mb-2">{n.fonte} — {n.dataPublicacao}</p>
          <a href="#" on:click|preventDefault={() => abrirModal(n)} class="text-yellow-400 hover:underline">
            🔎 Ver Detalhes
          </a>
        </div>
      {/each}
    </div>
  {:else if !carregando}
    <p class="text-sm text-slate-400 text-center">Nenhuma notícia encontrada com os filtros selecionados.</p>
  {/if}

  {#if modalAberto}
    <div class="fixed inset-0 bg-black bg-opacity-70 flex items-center justify-center z-50">
      <div class="bg-slate-900 p-6 rounded-lg max-w-xl w-full shadow-xl border border-yellow-400">
        <h2 class="text-xl font-bold mb-2">{noticiaSelecionada.titulo}</h2>
        <p class="text-sm text-gray-400 mb-2">{noticiaSelecionada.fonte} — {noticiaSelecionada.dataPublicacao}</p>
        <a href={noticiaSelecionada.link} target="_blank" class="text-blue-400 hover:underline mb-4 inline-block">🔗 Ver notícia original</a>
        <button on:click={fecharModal}
          class="mt-4 bg-yellow-500 hover:bg-yellow-600 text-white px-4 py-2 rounded font-bold w-full">
          Fechar
        </button>
      </div>
    </div>
  {/if}
</main>
