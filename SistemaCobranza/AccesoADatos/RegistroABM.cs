﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoADatos
{
    public static class RegistroABM
    {
        // TODO  verificar conexion y clase Registro 
        
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Inserta un registro en la Base de Datos
        /// </summary>
        /// <param name="registroX">Recibe un objeto de la Clase Registro</param>
        public static void InsertarRegistro(Registro registroX)
        {

            try
            {
                Conexion con = new Conexion();
                string sql = @"Inset into alumno(fechaHora,observacion,resultado,dni,cuit,id_usuario) values(@FechaHora,@Observacion,@Resultado,@Dni,@Cuit,@Id_usuario)";
                con.Conectar();

                var cmd = new MySqlCommand(sql, con.cn);
                cmd.Parameters.AddWithValue("@FechaHora", registroX.FechaHora);
                cmd.Parameters.AddWithValue("@Observacion", registroX.Observacion);
                cmd.Parameters.AddWithValue("@Resultado", registroX.Resultado);
                cmd.Parameters.AddWithValue("@Dni", registroX.Deudor.Dni);
                cmd.Parameters.AddWithValue("@Cuit", registroX.Empresa.Cuit);
                cmd.Parameters.AddWithValue("@Id_usuario", registroX.Usuario.Id_usuario);
                cmd.ExecuteNonQuery();

                con.Desconectar();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Busca registros por el atributo indicado y su correspondiente valor. Ej: RegsitroPorAtributo(Dni,25655846) 
        /// </summary>
        /// <param name="atributo">Atributo puede tomar: id_registo, dni, cuit, id_usuario</param>
        /// <param name="valor">valor de dicho atributo</param>
        /// <returns>Lista de registros</returns>
        public static List<Registro> RegistrosPorAtributo(string atributo, string valor)
        {
            try
            {
                List<Registro> Registros = new List<Registro>();
                Registro registroX;
                Conexion con = new Conexion();
                con.Conectar();

                string sql = "select * from Registro where " + atributo + "='" + valor + "'" ; // falta probar si funciona con int
                var cmd = new MySqlCommand(sql, con.cn);
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    registroX = new Registro();
                    registroX.Id_Registro = dr.GetInt32("cuit");
                    registroX.FechaHota = dr.GetDateTime("fechaHora");
                    registroX.Observacion = dr.GetString("observacion");
                    registroX.Resultado = dr.GetBoolean("valor");
                    registroX.Deudor = DeudorABM.DeudorPorDni( dr.GetString("dni"));
                    registroX.Empresa = EmpresaABM.EmpresaPorCuit(dr.GetString("cuit"));
                    registroX.Usuario = UsuarioABM.UsuarioPorId(dr.GetString("id_usuario"));
                    Registros.Add(registroX);
                }
                dr.Close();
                con.Desconectar();
                
                return Registros;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }


    }
    }
}