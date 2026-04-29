-- ============================================
-- Script de inicialização do PostgreSQL
-- Cria os bancos e usuários para todos os serviços
-- ============================================
CREATE DATABASE "tech-challenge";

\connect "tech-challenge"
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";