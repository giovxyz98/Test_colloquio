CREATE TABLE PartiCorpo (
    Id   INT PRIMARY KEY IDENTITY,
    Nome NVARCHAR(100) NOT NULL
);

CREATE TABLE Ambulatori (
    Id      INT PRIMARY KEY IDENTITY,
    Nome    NVARCHAR(100) NOT NULL,
    Piano   NVARCHAR(20),
    Interno NVARCHAR(10),
    Attivo  BIT NOT NULL DEFAULT 1
);

CREATE TABLE Esami (
    Id                 INT PRIMARY KEY IDENTITY,
    CodiceMinisteriale NVARCHAR(10)  NOT NULL,
    CodiceInterno      NVARCHAR(10)  NOT NULL,
    DescrizioneEsame   NVARCHAR(100) NOT NULL,
    IdParteCorpo       INT NOT NULL REFERENCES PartiCorpo(Id),
    DurataMinuti       INT,
    Attivo             BIT NOT NULL DEFAULT 1
);

CREATE TABLE EsamiAmbulatoriMtoM (
    IdEsame       INT NOT NULL REFERENCES Esami(Id),
    IdAmbulatorio INT NOT NULL REFERENCES Ambulatori(Id),
    PRIMARY KEY (IdEsame, IdAmbulatorio)
);

CREATE TABLE Pazienti (
    Id            INT PRIMARY KEY IDENTITY,
    Nome          NVARCHAR(100) NOT NULL,
    Cognome       NVARCHAR(100) NOT NULL,
    CodiceFiscale NVARCHAR(16),
    DataNascita   DATE,
    Sesso         CHAR(1),
    Telefono      NVARCHAR(20),
    Email         NVARCHAR(100),
    Indirizzo     NVARCHAR(200),
    Note          NVARCHAR(500)
);

CREATE TABLE Prenotazioni (
    Id            INT PRIMARY KEY IDENTITY,
    IdPaziente    INT NOT NULL REFERENCES Pazienti(Id),
    IdEsame       INT NOT NULL,
    IdAmbulatorio INT NOT NULL,
    DataOra       DATETIME NOT NULL,
    Stato         NVARCHAR(20) NOT NULL DEFAULT 'Prenotato',
    Note          NVARCHAR(500),
    DataCreazione DATETIME NOT NULL DEFAULT GETDATE(),
    DataModifica  DATETIME,

    CONSTRAINT FK_Prenotazioni_EsamiAmbulatori
        FOREIGN KEY (IdEsame, IdAmbulatorio)
        REFERENCES EsamiAmbulatoriMtoM(IdEsame, IdAmbulatorio)
);