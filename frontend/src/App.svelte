<script>
  let termo = '';
  let dataInicio = '';
  let dataFim = '';
  let noticias = [];
  let carregando = false;

  async function buscar() {
    const params = new URLSearchParams();

    if (termo) params.append("termo", termo);
    if (dataInicio) params.append("dataInicio", dataInicio);
    if (dataFim) params.append("dataFim", dataFim);

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

<main class="min-h-screen p-6 text-center max-w-3xl mx-auto">
  <h1 class="text-3xl font-bold mb-6">📰 TerraArte Notícias</h1>

  <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
    <input type="text" bind:value={termo} placeholder="Palavra-chave"
      class="p-2 rounded bg-slate-800 border border-slate-600" />

    <input type="date" bind:value={dataInicio}
      class="p-2 rounded bg-slate-800 border border-slate-600" />

    <input type="date" bind:value={dataFim}
      class="p-2 rounded bg-slate-800 border border-slate-600" />
  </div>

  <button
    class="bg-yellow-500 hover:bg-yellow-600 text-white font-bold px-6 py-2 rounded shadow mb-6 transition"
    on:click={buscar}>
    Buscar Notícias
  </button>

  {#if carregando}
    <p class="text-slate-300">🔄 Carregando...</p>
  {:else if noticias.length > 0}
    <div class="space-y-4 text-left">
      {#each noticias as n}
        <div class="bg-slate-800 p-4 rounded shadow">
          <h2 class="text-xl font-bold">{n.titulo}</h2>
          <p class="text-sm text-gray-400">{n.fonte} — {n.dataPublicacao}</p>
          <a href={n.link} class="text-blue-400 underline" target="_blank">🔗 Ver Notícia</a>
        </div>
      {/each}
    </div>
  {:else}
    <p class="text-sm text-slate-400 mt-4">Nenhuma notícia encontrada.</p>
  {/if}
</main>
