terraform {
  required_version = ">= 1.5.0"

  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.31"
    }
  }
}

provider "kubernetes" {
  config_path = var.kubeconfig_path
}

resource "kubernetes_namespace" "tech_challenge" {
  metadata {
    name = "tech-challenge"
  }
}

resource "kubernetes_secret" "db_secret" {
  metadata {
    name      = "tech-challenge-db-secret"
    namespace = kubernetes_namespace.tech_challenge.metadata[0].name
  }

  data = {
    POSTGRES_USER     = "postgres"
    POSTGRES_PASSWORD = "postgres"
    POSTGRES_DB       = "tech-challenge"
  }
}

resource "kubernetes_secret" "api_secret" {
  metadata {
    name      = "tech-challenge-api-secret"
    namespace = kubernetes_namespace.tech_challenge.metadata[0].name
  }

  data = {
    ConnectionStrings__DefaultConnection = "Host=tech-challenge-db;Port=5432;Database=tech-challenge;Username=postgres;Password=postgres"
  }
}

variable "kubeconfig_path" {
  description = "Caminho para o kubeconfig do cluster"
  type        = string
  default     = "~/.kube/config"
}
