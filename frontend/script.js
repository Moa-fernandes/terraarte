document.addEventListener("DOMContentLoaded", () => {
  const audio = new Audio("https://cdn.pixabay.com/audio/2022/03/15/audio_a2d5ba962e.mp3");
  audio.loop = true;
  audio.volume = 0.2;
  document.body.addEventListener("click", () => {
    if (audio.paused) audio.play();
  }, { once: true });

  document.getElementById("toggleDark").addEventListener("click", () => {
    document.documentElement.classList.toggle("dark");
  });

  // Harvard
  document.getElementById("fetchHarvard").addEventListener("click", async () => {
    const res = await fetch("https://api.harvardartmuseums.org/object?apikey=005b0290-bfa5-40c2-b1b0-eb152c1f8659&size=1&hasimage=1&sort=random");
    const json = await res.json();
    const obj = json.records?.[0];
    if (!obj) return;
    document.getElementById("harvardImage").src = obj.primaryimageurl;
    document.getElementById("harvardTitle").textContent = obj.title;
    document.getElementById("harvardDesc").textContent = obj.medium || obj.technique || "Sem descrição.";
    document.getElementById("harvardLink").href = obj.url;
    document.getElementById("harvardResult").classList.remove("hidden");
  });

  // Google Earth
  document.getElementById("fetchEarth").addEventListener("click", async () => {
    const lat = -22.9068;
    const lon = -43.1729;
    const d = "2020-05-01";
    const url = `https://api.nasa.gov/planetary/earth/imagery?lon=${lon}&lat=${lat}&date=${d}&dim=0.1&api_key=DEMO_KEY`;
    try {
      const resp = await fetch(url);
      const blob = await resp.blob();
      const imgUrl = URL.createObjectURL(blob);
      document.getElementById("earthImage").src = imgUrl;
      document.getElementById("earthDate").textContent = `Imagem próxima de ${d} (Rio de Janeiro)`;
      document.getElementById("earthLink").href = "https://earth.google.com/web/";
      document.getElementById("earthResult").classList.remove("hidden");
    } catch (e) {
      alert("Erro ao buscar imagem do satélite.");
    }
  });
});