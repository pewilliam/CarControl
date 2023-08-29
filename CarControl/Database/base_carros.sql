-- Criação do esquema
CREATE SCHEMA IF NOT EXISTS carcontrol;
ALTER DATABASE base_carros SET search_path TO carcontrol;

-- Tabela usuario
CREATE TABLE usuario (
  idusuario SERIAL PRIMARY KEY,
  nome VARCHAR(45) NOT NULL,
  login VARCHAR(15) NOT NULL,
  senha VARCHAR(20) NOT NULL,
  email VARCHAR(45)
);

--CREATE DOMAIN CPF
CREATE DOMAIN carcontrol.cpf AS TEXT
  CHECK (VALUE IS NULL OR (length(VALUE) = 11 AND VALUE ~ '^\d{11}$'));

-- Tabela carro
CREATE TABLE carro (
  idcarro SERIAL PRIMARY KEY,
  nome VARCHAR(45) NOT NULL
);

-- Tabela fabricante
CREATE TABLE fabricante (
  idfabricante SERIAL PRIMARY KEY,
  nome VARCHAR(30) NOT NULL
);

-- Tabela categoria
CREATE TABLE categoria (
  idcategoria SERIAL PRIMARY KEY,
  nome VARCHAR(30) NOT NULL
);

-- Tabela modelo
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

--Tabela cliente
CREATE TABLE IF NOT EXISTS carcontrol.cliente (
  idcliente SERIAL PRIMARY KEY,
  nome VARCHAR(45) NOT NULL,
  cpf cpf UNIQUE,
  email VARCHAR(45),
  dtnascimento DATE
);

--Tabela forma pagto
CREATE TABLE IF NOT EXISTS carcontrol.formapagto (
  idformapagto SERIAL PRIMARY KEY,
  nome VARCHAR(45) NOT NULL
);

--Tabela aluguel
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

--Tabela devolução
CREATE TABLE IF NOT EXISTS carcontrol.devolucao (
  iddevolucao SERIAL PRIMARY KEY,
  dhdevolucao TIMESTAMPTZ NOT NULL DEFAULT current_timestamp,
  idmodelo INT NOT NULL REFERENCES carcontrol.modelo(idmodelo),
  idcliente INT NOT NULL REFERENCES carcontrol.cliente(idcliente),
  idaluguel INT NOT NULL REFERENCES carcontrol.aluguel(idaluguel)
);

-- Criação da view vw_carro_modelo
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
    m.disponivel -- Adicionando a coluna "disponivel"
FROM carro c
LEFT JOIN modelo m ON m.idcarro = c.idcarro
LEFT JOIN categoria cat ON cat.idcategoria = m.idcategoria
LEFT JOIN fabricante f ON f.idfabricante = m.idfabricante;
-------------------------------------------------------------------------------------------------------------------------------------------------------------------

--Aluguel view--
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

-------------------------------------------------------------------------------------------------------------------------------------------------------------------
--Trigger para ao dar insert ou update na tabela aluguel, efetuar o cálculo do valor total considerando o valor por dia do modelo x quantidade de dias do aluguel
CREATE OR REPLACE FUNCTION calcular_valor_total()
RETURNS TRIGGER AS $$
DECLARE
    preco_dia numeric(18,2);
BEGIN
    -- Obtém o valor da diária do modelo alugado
    SELECT precodia INTO preco_dia
    FROM modelo
    WHERE idmodelo = NEW.idmodelo;

    -- Calcula o valor total do aluguel com base na diária e na quantidade de dias
    NEW.valoraluguel = preco_dia * NEW.diasaluguel;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER calcular_valor_total_trigger
BEFORE INSERT OR UPDATE OF diasaluguel, idmodelo ON aluguel
FOR EACH ROW
EXECUTE FUNCTION calcular_valor_total();


-------------------------------------------------------------------------------------------------------------------------------------------------------------------

--Trigger para atualizar status do aluguel ao efetuar devolução do mesmo
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

CREATE TRIGGER atualizar_flags_devolucao_trigger
AFTER INSERT ON devolucao
FOR EACH ROW
EXECUTE FUNCTION atualizar_flags_devolucao();

--Trigger para atualizar status de disponibilidade dos modelos ao efetuar devolução
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
-------------------------------------------------------------------------------------------------------------------------------------------------------------------

--Impede efetuar aluguel de modelo já alugado
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


-------------------------------------------------------------------------------------------------------------------------------------------------------------------
--Impede alterar preço do modelo enquanto aluguel estiver ativo
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

--CREATE FUNCTION TO VALIDATE CPF
CREATE OR REPLACE FUNCTION is_valid_cpf(input_cpf TEXT) RETURNS BOOLEAN AS $$
DECLARE
    cleaned_cpf TEXT;
    digits INT[11];
    i INT;
    sum1 INT = 0;
    sum2 INT = 0;
    remainder INT;
BEGIN
    -- Remove caracteres não numéricos do CPF
    cleaned_cpf := regexp_replace(input_cpf, '[^\d]', '', 'g');

    -- Se o CPF não possui 11 dígitos, não é válido
    IF length(cleaned_cpf) != 11 THEN
        RETURN FALSE;
    END IF;

    -- Converte o CPF em um array de dígitos inteiros
    FOR i IN 1..11 LOOP
        digits[i] := substring(cleaned_cpf, i, 1)::INT;
    END LOOP;

    -- Realiza a validação do CPF
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

--Verifica validade do CPF
CREATE OR REPLACE FUNCTION carcontrol.cpf_valido(_cpf text)
  RETURNS boolean AS
$BODY$
BEGIN
    RETURN (_cpf ~ E'^\\d{11}$');
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION carcontrol.cpf_valido(text) SET search_path=carcontrold;

ALTER FUNCTION carcontrol.cpf_valido(text)
  OWNER TO postgres;

ALTER SEQUENCE carcontrol.carro_idcarro_seq RESTART WITH 1;
ALTER SEQUENCE carcontrol.usuario_idusuario_seq RESTART WITH 1;
ALTER SEQUENCE carcontrol.modelo_idmodelo_seq RESTART WITH 1;
ALTER SEQUENCE carcontrol.fabricante_idfabricante_seq RESTART WITH 1;
ALTER SEQUENCE carcontrol.categoria_idcategoria_seq RESTART WITH 1;
ALTER SEQUENCE carcontrol.aluguel_idaluguel_seq RESTART WITH 1;
ALTER SEQUENCE carcontrol.devolucao_iddevolucao_seq RESTART WITH 1;
ALTER SEQUENCE carcontrol.formapagto_idformapagto_seq RESTART WITH 1;
ALTER SEQUENCE carcontrol.cliente_idcliente_seq RESTART WITH 1;