using CarControl.Tools;
using CarControl.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CarControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        LoginWindow loginwindow = new LoginWindow();
        NpgsqlConnection conn = new NpgsqlConnection();

        public MainWindow()
        {
            loginwindow.ShowDialog();
            conn = loginwindow.conn;
            if (conn.State == ConnectionState.Open)
            {
                CreateDatabase(conn);
                IniciaRelogio();
                InitializeComponent();
                CurrentUserTxb.Text = "Usuário: " + conn.UserName.ToString();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void MostrarCarroWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            CarrosWindow carrosWindow = new CarrosWindow(conn);
            carrosWindow.ShowDialog();
            carrosWindow.Owner = this;
        }

        private void ModelosCarroWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            ModelosWindow modelosWindow = new ModelosWindow(conn);
            modelosWindow.ShowDialog();
            modelosWindow.Owner = this;
        }

        private void ClientesWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientesWindow clientesWindow = new ClientesWindow(conn);
            clientesWindow.ShowDialog();
            clientesWindow.Owner = this;
        }

        private void AluguelWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            AluguelWindow aluguelWindow = new AluguelWindow(conn);
            aluguelWindow.ShowDialog();
            aluguelWindow.Owner = this;
        }

        private void DevolucaoWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            DevolucoesWindow devolucoesWindow = new DevolucoesWindow(conn);
            devolucoesWindow.ShowDialog();
            devolucoesWindow.Owner = this;
        }

        private void FabricantesCarroWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            FabricantesWindow fabricantesWindow = new FabricantesWindow(conn);
            fabricantesWindow.ShowDialog();
            fabricantesWindow.Owner = this;
        }

        private void CategoriasCarroWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            CategoriasWindows categoriasWindows = new CategoriasWindows(conn);
            categoriasWindows.ShowDialog();
            categoriasWindows.Owner = this;
        }

        private void FormasPagtoWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            FormasPagtoWindow formasPagtoWindow = new FormasPagtoWindow(conn);
            formasPagtoWindow.ShowDialog();
            formasPagtoWindow.Owner = this;
        }

        private void IniciaRelogio()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Tickevent;
            timer.Start();
        }

        private void Tickevent(object sender, EventArgs e)
        {
            ClockLabel.Text = DateTime.Now.ToString();
        }

        private void UsuariosWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            UsuariosWindow usuariosWindow = new(conn);
            usuariosWindow.ShowDialog();
            usuariosWindow.Owner = this;
        }

        private void TrocarUsuarioBtn_Click(object sender, RoutedEventArgs e)
        {
            conn.Close();
            LoginWindow loginwindow = new LoginWindow();
            loginwindow.ShowDialog();

            if (loginwindow.conn.UserName != null)
            {
                conn = loginwindow.conn;
                CurrentUserTxb.Text = "Usuário: " + conn.UserName.ToString();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.C) && (Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                MostrarCarroWindowBtn_Click(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.M) && (Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                ModelosCarroWindowBtn_Click(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.F) && (Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                FabricantesCarroWindowBtn_Click(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.A) && (Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                CategoriasCarroWindowBtn_Click(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.O) && (Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                FormasPagtoWindowBtn_Click(sender, e);
            }
        }

        static void CreateDatabase(NpgsqlConnection conn)
        {
            string sqlScript = @"
CREATE SCHEMA IF NOT EXISTS carcontrol;
ALTER DATABASE base_carros SET search_path TO carcontrol;

CREATE TABLE usuario (
  idusuario SERIAL PRIMARY KEY,
  nome VARCHAR(45) NOT NULL,
  login VARCHAR(15) NOT NULL,
  senha VARCHAR(20) NOT NULL,
  email VARCHAR(45)
);

CREATE DOMAIN carcontrol.cpf AS TEXT
  CHECK (VALUE IS NULL OR (length(VALUE) = 11 AND VALUE ~ '^\d{11}$'));

CREATE TABLE carro (
  idcarro SERIAL PRIMARY KEY,
  nome VARCHAR(45) NOT NULL
);

CREATE TABLE fabricante (
  idfabricante SERIAL PRIMARY KEY,
  nome VARCHAR(30) NOT NULL
);

CREATE TABLE categoria (
  idcategoria SERIAL PRIMARY KEY,
  nome VARCHAR(30) NOT NULL
);

CREATE TABLE modelo (
  idmodelo SERIAL PRIMARY KEY,
  nome VARCHAR(45) NOT NULL,
  cor VARCHAR(45) NOT NULL,
  qtdportas INT NOT NULL,
  qtdpassageiros INT NOT NULL,
  combustivel VARCHAR(45) NOT NULL,
  placa VARCHAR(7) NOT NULL,
  ano VARCHAR(4) NOT NULL,
  tipocambio VARCHAR(20) NOT NULL,
  precodia DECIMAL NOT NULL,
  idcarro INT NOT NULL,
  idfabricante INT NOT NULL,
  idcategoria INT NOT NULL,
  disponivel boolean DEFAULT true,
  FOREIGN KEY (idcarro) REFERENCES carro(idcarro) ON DELETE NO ACTION ON UPDATE NO ACTION,
  FOREIGN KEY (idfabricante) REFERENCES fabricante(idfabricante) ON DELETE NO ACTION ON UPDATE NO ACTION,
  FOREIGN KEY (idcategoria) REFERENCES categoria(idcategoria) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE IF NOT EXISTS carcontrol.cliente (
  idcliente SERIAL PRIMARY KEY,
  nome VARCHAR(45) NOT NULL,
  cpf cpf UNIQUE,
  email VARCHAR(45),
  dtnascimento DATE
);

CREATE TABLE IF NOT EXISTS carcontrol.formapagto (
  idformapagto SERIAL PRIMARY KEY,
  nome VARCHAR(45) NOT NULL
);

CREATE TABLE IF NOT EXISTS carcontrol.aluguel (
  idaluguel SERIAL PRIMARY KEY,
  idcliente INT NOT NULL REFERENCES carcontrol.cliente(idcliente),
  idmodelo INT NOT NULL REFERENCES carcontrol.modelo(idmodelo),
  idformapagto INT NOT NULL REFERENCES carcontrol.formapagto(idformapagto),
  dhaluguel TIMESTAMPTZ NOT NULL DEFAULT current_timestamp,
  diasaluguel INT NOT NULL,
  valoraluguel DECIMAL(18,2) NOT NULL,
  em_andamento boolean DEFAULT true
);

CREATE TABLE IF NOT EXISTS carcontrol.devolucao (
  iddevolucao SERIAL PRIMARY KEY,
  dhdevolucao TIMESTAMPTZ NOT NULL DEFAULT current_timestamp,
  idmodelo INT NOT NULL REFERENCES carcontrol.modelo(idmodelo),
  idcliente INT NOT NULL REFERENCES carcontrol.cliente(idcliente),
  idaluguel INT NOT NULL REFERENCES carcontrol.aluguel(idaluguel)
);

CREATE OR REPLACE VIEW carcontrol.vw_carro_modelo
AS
SELECT
    m.idmodelo,
    c.idcarro,
    c.nome AS nome_carro,
    m.nome AS nome_modelo,
    f.nome AS fabricante,
    cat.nome AS categoria,
    m.cor,
    m.qtdportas,
    m.qtdpassageiros,
    m.combustivel,
    m.placa,
    m.ano,
    m.tipocambio,
    m.precodia AS preco,
    m.disponivel
FROM carro c
LEFT JOIN modelo m ON m.idcarro = c.idcarro
LEFT JOIN categoria cat ON cat.idcategoria = m.idcategoria
LEFT JOIN fabricante f ON f.idfabricante = m.idfabricante;

CREATE VIEW vw_aluguel AS
SELECT 
    a.idaluguel,
    a.idcliente,
    c.nome AS nome_cliente,
    a.idmodelo,
    m.nome AS nome_modelo,
    a.idformapagto,
    f.nome AS forma_pagto,
    dhaluguel,
    diasaluguel,
    valoraluguel
FROM
    aluguel a
LEFT JOIN
    cliente c ON c.idcliente = a.idcliente
LEFT JOIN
    modelo m ON m.idmodelo = a.idmodelo
LEFT JOIN
    formapagto f ON f.idformapagto = a.idformapagto;

CREATE OR REPLACE FUNCTION calcular_valor_total()
RETURNS TRIGGER AS $$
DECLARE
    preco_dia numeric(18,2);
BEGIN
    SELECT precodia INTO preco_dia
    FROM modelo
    WHERE idmodelo = NEW.idmodelo;

    NEW.valoraluguel = preco_dia * NEW.diasaluguel;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER calcular_valor_total_trigger
BEFORE INSERT OR UPDATE OF diasaluguel, idmodelo ON aluguel
FOR EACH ROW
EXECUTE FUNCTION calcular_valor_total();

CREATE OR REPLACE FUNCTION atualizar_flags_devolucao()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE aluguel
    SET em_andamento = false
    WHERE idaluguel = NEW.idaluguel;

    UPDATE modelo
    SET disponivel = true
    WHERE idmodelo = NEW.idmodelo;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION verificar_idade_cliente()
RETURNS TRIGGER AS $$
DECLARE
    idade_cliente integer;
BEGIN
    -- Calcula a idade do cliente com base na diferença entre a data atual e a data de nascimento
    idade_cliente := DATE_PART('year', AGE(NEW.dtnascimento));

    IF idade_cliente < 18 THEN
        RAISE EXCEPTION 'Não é permitido cadastrar clientes menores de 18 anos.';
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER verificar_idade_cliente_trigger
BEFORE INSERT ON cliente
FOR EACH ROW
EXECUTE FUNCTION verificar_idade_cliente();

CREATE TRIGGER atualizar_flags_devolucao_trigger
AFTER INSERT ON devolucao
FOR EACH ROW
EXECUTE FUNCTION atualizar_flags_devolucao();

CREATE OR REPLACE FUNCTION atualizar_modelo_indisponivel()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE modelo
    SET disponivel = false
    WHERE idmodelo = NEW.idmodelo;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER atualizar_modelo_indisponivel_trigger
AFTER INSERT OR UPDATE OF idmodelo ON aluguel
FOR EACH ROW
EXECUTE FUNCTION atualizar_modelo_indisponivel();

CREATE OR REPLACE FUNCTION verificar_disponibilidade_modelo()
RETURNS TRIGGER AS $$
DECLARE
    modelo_disponivel boolean;
BEGIN
    SELECT disponivel INTO modelo_disponivel
    FROM modelo
    WHERE idmodelo = NEW.idmodelo;

    IF modelo_disponivel = false THEN
        RAISE EXCEPTION 'Não é possível inserir este modelo em um aluguel, pois está em um aluguel ativo no momento.';
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER verificar_disponibilidade_modelo_trigger
BEFORE INSERT ON aluguel
FOR EACH ROW
EXECUTE FUNCTION verificar_disponibilidade_modelo();

CREATE OR REPLACE FUNCTION verificar_aluguel_ativo()
RETURNS TRIGGER AS $$
DECLARE
    aluguel_ativo boolean;
BEGIN
    SELECT em_andamento INTO aluguel_ativo
    FROM aluguel
    WHERE idmodelo = NEW.idmodelo AND em_andamento = true;

    IF aluguel_ativo THEN
        IF OLD.precodia <> NEW.precodia THEN
            RAISE EXCEPTION 'Não é possível alterar o preço do modelo enquanto um aluguel estiver ativo.';
        END IF;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER verificar_aluguel_ativo_trigger
BEFORE UPDATE ON modelo
FOR EACH ROW
EXECUTE FUNCTION verificar_aluguel_ativo();

CREATE OR REPLACE FUNCTION is_valid_cpf(input_cpf TEXT) RETURNS BOOLEAN AS $$
DECLARE
    cleaned_cpf TEXT;
    digits INT[11];
    i INT;
    sum1 INT = 0;
    sum2 INT = 0;
    remainder INT;
BEGIN
    cleaned_cpf := regexp_replace(input_cpf, '[^\d]', '', 'g');

    IF length(cleaned_cpf) != 11 THEN
        RETURN FALSE;
    END IF;

    FOR i IN 1..11 LOOP
        digits[i] := substring(cleaned_cpf, i, 1)::INT;
    END LOOP;

    FOR i IN 1..9 LOOP
        sum1 := sum1 + digits[i] * (11 - i);
        sum2 := sum2 + digits[i] * (12 - i);
    END LOOP;

    remainder := sum1 % 11;
    IF remainder < 2 THEN
        IF digits[10] != 0 THEN
            RETURN FALSE;
        END IF;
    ELSE
        IF digits[10] != 11 - remainder THEN
            RETURN FALSE;
        END IF;
    END IF;

    sum2 := sum2 + digits[9] * 2;
    remainder := sum2 % 11;
    IF remainder < 2 THEN
        IF digits[11] != 0 THEN
            RETURN FALSE;
        END IF;
    ELSE
        IF digits[11] != 11 - remainder THEN
            RETURN FALSE;
        END IF;
    END IF;

    RETURN TRUE;
END;
$$ LANGUAGE plpgsql;

INSERT INTO categoria (nome)
VALUES
    ('ESPORTIVO'),
    ('SEDAN'),
    ('SUV'),
    ('HATCHBACK'),
    ('UTILITÁRIO');

INSERT INTO fabricante (nome)
VALUES
    ('TOYOTA'),
    ('NISSAN'),
    ('FORD'),
    ('CHEVROLET'),
    ('HONDA');

INSERT INTO carcontrol.formapagto (idformapagto, nome)
VALUES (1, 'DINHEIRO'),
       (2, 'PIX'),
       (3, 'CARTÃO DE CRÉDITO'),
       (4, 'CARTÃO DE DÉBITO');
            ";

            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(sqlScript, conn, transaction))
                    {
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }
    }
}
