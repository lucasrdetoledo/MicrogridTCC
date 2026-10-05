(function () {
    const canvas = document.getElementById("energyHistoryChart");
    if (!canvas || typeof Chart === "undefined") {
        return;
    }

    const labels = JSON.parse(canvas.dataset.labels);
    const solar = JSON.parse(canvas.dataset.solar);
    const consumption = JSON.parse(canvas.dataset.consumption);

    new Chart(canvas, {
        type: "line",
        data: {
            labels,
            datasets: [
                {
                    label: "Geração solar (W)",
                    data: solar,
                    borderColor: "#f5c542",
                    backgroundColor: "rgba(245, 197, 66, 0.18)",
                    fill: true,
                    tension: 0.35,
                    pointRadius: 2
                },
                {
                    label: "Consumo total (W)",
                    data: consumption,
                    borderColor: "#4cc3ff",
                    backgroundColor: "rgba(76, 195, 255, 0.12)",
                    fill: true,
                    tension: 0.35,
                    pointRadius: 2
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    labels: { color: "#e8f1fb" }
                }
            },
            scales: {
                x: {
                    ticks: { color: "#9bb0c6" },
                    grid: { color: "rgba(148, 197, 255, 0.08)" }
                },
                y: {
                    beginAtZero: true,
                    ticks: { color: "#9bb0c6" },
                    grid: { color: "rgba(148, 197, 255, 0.08)" }
                }
            }
        }
    });
})();
