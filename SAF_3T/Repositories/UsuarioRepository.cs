﻿using SAF_3T.Contexts;
using SAF_3T.Domains;
using SAF_3T.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAF_3T.Utils;
using Microsoft.EntityFrameworkCore;

namespace SAF_3T.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private SAFContext ctx = new SAFContext();

        public object BCrypt { get; private set; }

        public void AlterarSenha(int idRecebido, Usuario novaSenha)
        {
            Usuario usuarioLogado = ctx.Usuarios.FirstOrDefault(u => u.IdUsuario == idRecebido);

            usuarioLogado.Senha = Criptografia.GerarHash(novaSenha.Senha);
            ctx.Update(usuarioLogado);
            ctx.SaveChanges();
        }

        public Usuario BuscarPorCPF(string CPFUsuario)
        {
            return ctx.Usuarios.FirstOrDefault(u => u.Cpf == CPFUsuario);
        }

        public Usuario BuscarPorId(int idUsuario)
        {
           return ctx.Usuarios.FirstOrDefault(c => c.IdUsuario == idUsuario);
        }

        public Usuario BuscarPorNumero(string NumeroUsuario)
        {
            return ctx.Usuarios.FirstOrDefault(c => c.Telefone == NumeroUsuario);
        }

        public void Cadastrar(Usuario novoUsuario)
        {
            novoUsuario.Senha = Criptografia.GerarHash(novoUsuario.Senha);
            ctx.Usuarios.Add(novoUsuario);
            ctx.SaveChanges();
        }

        public void Deletar(int idUsuario)
        {
            var encontrar = ctx.Usuarios.FirstOrDefault(c => c.IdUsuario == idUsuario);
            ctx.Usuarios.Remove(encontrar);
            ctx.SaveChanges();
        }

        public List<Usuario> ListarTodos()
        {
            return ctx.Usuarios
                .AsNoTracking()
                .Include(u => u.IdTipoUsuarioNavigation)
                .ToList();
        }

        public Usuario Login(string CPF, string senha)
        {
            var usuario = ctx.Usuarios.FirstOrDefault(u => u.Cpf == CPF);

            if (usuario != null)
            {
                
                if (senha.Length != 32)
                {
                    var SenhaBanco = ctx.Usuarios.FirstOrDefault(u => u.Cpf == CPF && u.Senha == senha);

                    if (SenhaBanco != null)
                    {
                        SenhaBanco.Senha = Criptografia.GerarHash(SenhaBanco.Senha);

                        ctx.SaveChanges();

                        return usuario;
                    }
                }
                bool confere = Criptografia.Comparar(senha, usuario.Senha);

                if (confere)
                {
                    return usuario;
                }
            }

            return null;
        }
    }
}