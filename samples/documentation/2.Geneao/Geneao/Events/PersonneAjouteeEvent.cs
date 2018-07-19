﻿using CQELight.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Geneao.Events
{
    public sealed class PersonneAjouteeEvent : BaseDomainEvent
    {
        public string Nom { get; private set; }
        public string Prenom { get; private set; }
        public string LieuNaissance { get; private set; }
        public DateTime DateNaissance { get; private set; }

        private PersonneAjouteeEvent() { }

        internal PersonneAjouteeEvent(string nom, string prenom, string lieuNaissance, DateTime dateNaissance)
        {
            if (string.IsNullOrWhiteSpace(nom)) throw new ArgumentException("PersonneAjouteeEvent.Ctor() : Nom requis.", nameof(nom));

            if (string.IsNullOrWhiteSpace(prenom)) throw new ArgumentException("PersonneAjouteeEvent.Ctor() : Prénom requis.", nameof(prenom));

            if (string.IsNullOrWhiteSpace(lieuNaissance)) throw new ArgumentException("PersonneAjouteeEvent.Ctor() : Lieu naissance requis.", nameof(lieuNaissance));

            DateNaissance = dateNaissance;
            LieuNaissance = lieuNaissance;
            Prenom = prenom;
            Nom = nom;
        }
    }

}
