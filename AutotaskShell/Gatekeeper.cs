﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutotaskQueryService;

namespace AutotaskShell
{
    public class Gatekeeper
    {
        private readonly IQueryService _service;

        public Gatekeeper(IQueryService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _service = service;
        }

        public void TryLoginUntilSuccess()
        {
            while (!TryLogin())
                Console.WriteLine("Invalid login credentials.");
        }

        public bool TryLogin()
        {
            try
            {
                Login();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void Login()
        {
            string userName = GetUserName();
            string pass = GetPassword();
            _service.Login(userName, pass);
        }

        private static string GetUserName()
        {
            Console.Write("UserName: ");
            var userName = Console.ReadLine();

            return userName;
        }

        private static string GetPassword()
        {
            Console.Write("Password: ");
            var originalColor = Console.ForegroundColor;

            ChangeConsoleToPasswordMode();
            var pass = Console.ReadLine();
            RestoreNormalConsole(originalColor);

            return pass;
        }
        
        private static void ChangeConsoleToPasswordMode()
        {
            Console.ForegroundColor = Console.BackgroundColor;
            Console.CursorVisible = false;
        }

        private static void RestoreNormalConsole(ConsoleColor originalColor)
        {
            Console.ForegroundColor = originalColor;
            Console.CursorVisible = true;
        }
    }
}
