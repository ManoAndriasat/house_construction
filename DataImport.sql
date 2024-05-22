drop TABLE ImportDevis;
drop TABLE ImportMaison;
drop TABLE ImportPayement;

CREATE TABLE ImportDevis (
    client VARCHAR(50),
    ref_devis VARCHAR(50) PRIMARY KEY,
    type_maison VARCHAR(50),
    finition VARCHAR(50),
    taux_finition VARCHAR(50),
    date_devis VARCHAR(50),
    date_debut VARCHAR(50),
    lieu VARCHAR(50)
);
CREATE TABLE ImportMaison (
    type_maison VARCHAR(50),
    description VARCHAR(255),
    surface VARCHAR(50),
    code_travaux VARCHAR(50),
    type_travaux VARCHAR(50),
    unite VARCHAR(50),
    prix_unitaire VARCHAR(50),
    quantite VARCHAR(50),
    duree_travaux VARCHAR(50)
);
CREATE TABLE ImportPayement (
    ref_devis VARCHAR(50),
    ref_paiement VARCHAR(50),
    date_paiement VARCHAR(50),
    montant VARCHAR(50)
);
