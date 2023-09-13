-- Criação do esquema
BEGIN;
CREATE SCHEMA IF NOT EXISTS carcontrol;
ALTER DATABASE base_carros SET search_path TO carcontrol;
COMMIT;

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
  dhaluguel TIMESTAMP NOT NULL DEFAULT current_timestamp,
  diasaluguel INT NOT NULL,
  valoraluguel DECIMAL(18,2) NOT NULL,
  em_andamento boolean DEFAULT true
);

--Tabela devolução
CREATE TABLE IF NOT EXISTS carcontrol.devolucao (
  iddevolucao SERIAL PRIMARY KEY,
  dhdevolucao TIMESTAMP NOT NULL DEFAULT current_timestamp,
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

--vw-aluguel--
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
    valoraluguel,
    em_andamento
FROM
    aluguel a
LEFT JOIN
    cliente c ON c.idcliente = a.idcliente
LEFT JOIN
    modelo m ON m.idmodelo = a.idmodelo
LEFT JOIN
    formapagto f ON f.idformapagto = a.idformapagto;

--vw_devolucao
CREATE VIEW vw_devolucao AS
SELECT 
    iddevolucao,
	d.idmodelo,
	m.nome AS nome_modelo,
	d.idcliente,
	c.nome AS nome_cliente,
	idaluguel,
	dhdevolucao
FROM 
	devolucao d
LEFT JOIN
	modelo m ON m.idmodelo = d.idmodelo
LEFT JOIN
	cliente c ON c.idcliente = d.idcliente;

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

-------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TRIGGER calcular_valor_total_trigger
BEFORE INSERT OR UPDATE OF diasaluguel, idmodelo ON aluguel
FOR EACH ROW
EXECUTE FUNCTION calcular_valor_total();

CREATE OR REPLACE FUNCTION carcontrol.verificar_diasaluguel()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.diasaluguel = 0 THEN
        RAISE EXCEPTION 'O valor de diasaluguel não pode ser igual a 0';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER aluguel_valida_diasaluguel
BEFORE INSERT ON carcontrol.aluguel
FOR EACH ROW
EXECUTE FUNCTION carcontrol.verificar_diasaluguel();


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

CREATE TABLE IF NOT EXISTS carcontrol.recebimento
(
    idrecebimento serial,
    idaluguel integer NOT NULL,
    iddevolucao integer,
    valororiginal numeric(18,2) NOT NULL,
    valorrecebido numeric(18,2),
    dhrecebimento timestamp without time zone,
    em_aberto boolean NOT NULL DEFAULT true,
    recebimento_dia_previsto date,
    CONSTRAINT recebimento_pkey PRIMARY KEY (idrecebimento),
    CONSTRAINT recebimento_idaluguel_fkey FOREIGN KEY (idaluguel)
        REFERENCES carcontrol.aluguel (idaluguel) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT recebimento_iddevolucao_fkey FOREIGN KEY (iddevolucao)
        REFERENCES carcontrol.devolucao (iddevolucao) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE OR REPLACE FUNCTION carcontrol.criar_recebimento_em_aberto()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO carcontrol.recebimento (idaluguel, iddevolucao, valororiginal, valorrecebido, dhrecebimento, em_aberto, recebimento_dia_previsto)
    VALUES (NEW.idaluguel, NULL, NEW.valoraluguel, NULL, NULL, TRUE, NEW.dhaluguel + (NEW.diasaluguel || ' days')::interval);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Crie um gatilho para chamar a função ao inserir um aluguel
CREATE TRIGGER criar_recebimento_trigger
AFTER INSERT
ON carcontrol.aluguel
FOR EACH ROW
EXECUTE FUNCTION carcontrol.criar_recebimento_em_aberto();

CREATE OR REPLACE FUNCTION carcontrol.atualizar_recebimento_apos_devolucao()
RETURNS TRIGGER AS $$
DECLARE
    valor_atualizado numeric(18,2);
	valor_original numeric(18,2);
	preco_dia numeric(18,2);
	dh_aluguel date;
	recebimento_dia_previsto date;
BEGIN
	SELECT r.recebimento_dia_previsto, r.valororiginal
    INTO recebimento_dia_previsto, valor_original
    FROM recebimento r
    LEFT JOIN devolucao d ON d.idaluguel = r.idaluguel;
	
	SELECT m.precodia
	INTO preco_dia
	FROM modelo m
	LEFT JOIN devolucao d ON d.idmodelo = m.idmodelo;
	
	SELECT dhaluguel
	INTO dh_aluguel
	FROM aluguel a
	LEFT JOIN devolucao d ON a.idaluguel = d.idaluguel;

    -- Calcule o valor atualizado com base nas regras especificadas
    IF NEW.dhdevolucao::DATE = dh_aluguel THEN
		valor_atualizado := preco_dia;

    ELSIF recebimento_dia_previsto > NEW.dhdevolucao::date AND NEW.dhdevolucao <> dh_aluguel THEN
        valor_atualizado := preco_dia * (NEW.dhdevolucao::date - dh_aluguel);
        
    ELSIF recebimento_dia_previsto < NEW.dhdevolucao::date THEN
        valor_atualizado := valor_original + (preco_dia * (NEW.dhdevolucao::date - recebimento_dia_previsto));
		
    ELSE
		valor_atualizado := valor_original;
    END IF;

    -- Atualize o registro correspondente na tabela de recebimento
    UPDATE carcontrol.recebimento
    SET iddevolucao = NEW.iddevolucao,
		valorrecebido = valor_atualizado,
        dhrecebimento = NEW.dhdevolucao,
        em_aberto = FALSE
    WHERE idaluguel = NEW.idaluguel;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Crie um gatilho para chamar a função ao inserir uma devolução
CREATE TRIGGER atualizar_recebimento_apos_devolucao_trigger
AFTER INSERT
ON carcontrol.devolucao
FOR EACH ROW
EXECUTE FUNCTION carcontrol.atualizar_recebimento_apos_devolucao();
