
// Fonction pour générer le PDF
function generatePDF() {
    // Masquer le bouton "Exporter" avant de capturer le contenu
    $('#generatePDF').hide();

    // Ajuster la hauteur du conteneur proforma avant de le convertir en PDF
    var proformaContainer = document.getElementById('mytable');
    proformaContainer.style.height = 'auto'; // Réinitialiser la hauteur à 'auto'

    // Options pour html2canvas
    var options = {
        scale: 2,  // Ajuster l'échelle pour améliorer la résolution
        logging: true,
        useCORS: true,
        windowWidth: proformaContainer.scrollWidth,
        windowHeight: proformaContainer.scrollHeight
    };

    // Capture de l'élément avec l'ID 'proforma' en tant qu'image
    html2canvas(proformaContainer, options).then(function(canvas) {
        // Restaurer la hauteur du conteneur proforma à sa valeur d'origine
        proformaContainer.style.height = '';

        // Spécifier la largeur personnalisée pour le PDF (ajustez la valeur selon vos besoins)
        var pdfWidth = 210; // Largeur en millimètres (A4 par exemple)
        
        var imgData = canvas.toDataURL('image/png');

        var doc = new jsPDF({
            unit: 'mm',
            format: [pdfWidth, (pdfWidth / canvas.width) * canvas.height]
        });

        doc.addImage(imgData, 'PNG', 10, 10, pdfWidth - 90, 0);

        // Styles personnalisés pour le PDF
        doc.setFont("helvetica");
        doc.setFontSize(12);
        doc.setFillColor(255, 255, 255); // Fond blanc
        doc.setTextColor(0, 0, 0); // Texte noir

        // Sauvegarde du fichier PDF avec le nom 'bon.pdf'
        doc.save('bon.pdf');

        // Afficher à nouveau le bouton "Exporter" après la génération du PDF
        $('#generatePDF').show();
    });
}

// Appeler la fonction de génération de PDF lorsque le bouton est cliqué
$('#generatePDF').click(function () {
    generatePDF();
});