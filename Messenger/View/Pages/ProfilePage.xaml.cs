﻿using Messenger.ChatDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Messenger.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        ChatUser user = new ChatUser();
        public ProfilePage(ChatUser user)
        {
            InitializeComponent();
            this.user = user;

            loginField.txtLimitedInput.Text = user.Login;
            loginField.txtLimitedInput.IsEnabled = false;

            emailField.txtLimitedInput.Text = user.Email;
            emailField.txtLimitedInput.IsEnabled = false;

            aboutField.txtLimitedInput.Text = user.About;
            aboutField.txtLimitedInput.IsEnabled = false;
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            loginField.txtLimitedInput.IsEnabled = true;
            emailField.txtLimitedInput.IsEnabled = true;
            aboutField.txtLimitedInput.IsEnabled = true;

            SaveBtn.IsEnabled = true;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = loginField.txtLimitedInput.Text;
            string email = emailField.txtLimitedInput.Text;
            string about = aboutField.txtLimitedInput.Text;

            try
            {
                using (ChatDBModel db = new ChatDBModel())
                {
                    #region Валидация
                    bool IsValid = true;    // переменная равна true только, когда все поля валидны

                    // валидация логина
                    string loginError = ChatUser.LoginValidation(login);
                    // если есть ошибка, то есть метод LoginValidation не вернул null
                    if (loginError != null)
                    {
                        loginField.txtLimitedInput.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.2 };
                        loginField.txtLimitedInput.BorderBrush = new SolidColorBrush(Colors.Red);
                        loginField.txtLimitedInput.ToolTip = CreateToolTip(loginError);
                        IsValid = false;
                    }
                    // если валидно
                    else
                    {
                        loginField.txtLimitedInput.Background = Brushes.Transparent;
                        loginField.txtLimitedInput.BorderBrush = Brushes.Transparent;
                        loginField.txtLimitedInput.ToolTip = null;
                    }

                    // валидация email
                    string emailError = ChatUser.EmailValidation(email);
                    if (emailError != null)
                    {
                        emailField.txtLimitedInput.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.2 };
                        emailField.txtLimitedInput.BorderBrush = new SolidColorBrush(Colors.Red);
                        emailField.txtLimitedInput.ToolTip = CreateToolTip(emailError);
                        IsValid = false;
                    }
                    else
                    {
                        emailField.txtLimitedInput.Background = Brushes.Transparent;
                        emailField.txtLimitedInput.BorderBrush = Brushes.Transparent;
                        emailField.txtLimitedInput.ToolTip = null;
                    }
                    #endregion

                    if (IsValid)
                    {
                        db.Database.ExecuteSqlCommand("update ChatUser set Login='" +
                            loginField.txtLimitedInput.Text + "' where Id=" + user.Id);

                        db.Database.ExecuteSqlCommand("update ChatUser set Email='" +
                            emailField.txtLimitedInput.Text + "' where Id=" + user.Id);

                        db.Database.ExecuteSqlCommand("update ChatUser set About='" +
                            aboutField.txtLimitedInput.Text + "' where Id=" + user.Id);

                        db.SaveChanges();

                        // Popup о том, что данные сохранены
                        SavePopup.IsOpen = true;
                        // таймер на показ Popup на 2 секунды
                        DispatcherTimer time = new DispatcherTimer();
                        time.Interval = TimeSpan.FromSeconds(2);
                        time.Start();
                        time.Tick += delegate
                        {
                            SavePopup.IsOpen = false;
                            time.Stop();
                        };

                        // делаем поля недоступными для ввода
                        loginField.txtLimitedInput.IsEnabled = false;
                        emailField.txtLimitedInput.IsEnabled = false;
                        aboutField.txtLimitedInput.IsEnabled = false;

                        SaveBtn.IsEnabled = false;
                    }
                    else
                    {
                        // Popup о том, что надо исправить ошибки
                        ErrorPopup.IsOpen = true;
                        // таймер на показ Popup на 2 секунды
                        DispatcherTimer time = new DispatcherTimer();
                        time.Interval = TimeSpan.FromSeconds(2);
                        time.Start();
                        time.Tick += delegate
                        {
                            ErrorPopup.IsOpen = false;
                            time.Stop();
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("При попытке сохранить данные произошла ошибка.\nТекст ошибки:\t" + ex.Message);
                return;
            }

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            // делаем поле недоступным для ввода
            loginField.txtLimitedInput.IsEnabled = false;
            // убираем ошибки
            loginField.txtLimitedInput.Background = Brushes.Transparent;
            loginField.txtLimitedInput.BorderBrush = Brushes.Transparent;
            // записываем старые данные
            loginField.txtLimitedInput.Text = user.Login;

            emailField.txtLimitedInput.IsEnabled = false;
            emailField.txtLimitedInput.Background = Brushes.Transparent;
            emailField.txtLimitedInput.BorderBrush = Brushes.Transparent;
            emailField.txtLimitedInput.Text = user.Email;

            aboutField.txtLimitedInput.IsEnabled = false;
            aboutField.txtLimitedInput.Text = user.About;

            // кнопка сохранения недоступна
            SaveBtn.IsEnabled = false;
        }

        // подсказка для поля с не валидными данными
        private ToolTip CreateToolTip(string text)
        {
            // всплывающая подсказка для отображения текста ошибки
            ToolTip tooltip = new ToolTip();

            Style style = Application.Current.FindResource("errorEditToolTipStyle") as Style;
            tooltip.Style = style;
            tooltip.Content = text;

            return tooltip;
        }

        private void changePasswordTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            changePasswordGrid.Visibility = Visibility.Visible;
            changePasswordTextBlock.Visibility = Visibility.Collapsed;
        }

        private void CancelPassword_Click(object sender, RoutedEventArgs e)
        {
            oldPasswordField.txtLimitedInput.Text = "";
            oldPasswordField.txtLimitedInput.Background = Brushes.Transparent;
            oldPasswordField.txtLimitedInput.BorderBrush = Brushes.Transparent;

            newPasswordField.txtLimitedInput.Text = "";
            newPasswordField.txtLimitedInput.Background = Brushes.Transparent;
            newPasswordField.txtLimitedInput.BorderBrush = Brushes.Transparent;

            newPasswordRepeatField.txtLimitedInput.Text = "";
            newPasswordRepeatField.txtLimitedInput.Background = Brushes.Transparent;
            newPasswordRepeatField.txtLimitedInput.BorderBrush = Brushes.Transparent;

            changePasswordGrid.Visibility = Visibility.Collapsed;
            changePasswordTextBlock.Visibility = Visibility.Visible;
        }

        private void SavePassword_Click(object sender, RoutedEventArgs e)
        {
            string oldP = oldPasswordField.txtLimitedInput.Text;
            string newP = newPasswordField.txtLimitedInput.Text;
            string newRepeatP = newPasswordRepeatField.txtLimitedInput.Text;

            #region проверка, правильно ли введен старый пароль
            bool rightOldPassword = true;

            string wrongOldPasswordMsg = null;
            if (oldP != user.Password) wrongOldPasswordMsg = "Вы ввели неправильный пароль";

            if (wrongOldPasswordMsg != null)
            {
                oldPasswordField.txtLimitedInput.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.2 };
                oldPasswordField.txtLimitedInput.BorderBrush = new SolidColorBrush(Colors.Red);
                oldPasswordField.txtLimitedInput.ToolTip = CreateToolTip(wrongOldPasswordMsg);
                rightOldPassword = false;
            }
            else
            {
                oldPasswordField.txtLimitedInput.Background = Brushes.Transparent;
                oldPasswordField.txtLimitedInput.BorderBrush = Brushes.Transparent;
                oldPasswordField.txtLimitedInput.ToolTip = null;
            }
            #endregion

            if (rightOldPassword)
            {
                try
                {
                    using (ChatDBModel db = new ChatDBModel())
                    {
                        #region Совпадение паролей в поле "Новый пароль" и "Повторите новый пароль"
                        bool IsValid = true;    // переменная равна true только, когда все поля валидны

                        if(newP != newRepeatP)
                        {
                            newPasswordField.txtLimitedInput.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.2 };
                            newPasswordField.txtLimitedInput.BorderBrush = new SolidColorBrush(Colors.Red);
                            newPasswordField.txtLimitedInput.ToolTip = CreateToolTip("Пароли не совпадают");

                            newPasswordRepeatField.txtLimitedInput.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.2 };
                            newPasswordRepeatField.txtLimitedInput.BorderBrush = new SolidColorBrush(Colors.Red);
                            newPasswordRepeatField.txtLimitedInput.ToolTip = CreateToolTip("Пароли не совпадают");

                            IsValid = false;
                        }
                        else
                        {
                            newPasswordField.txtLimitedInput.Background = Brushes.Transparent;
                            newPasswordField.txtLimitedInput.BorderBrush = Brushes.Transparent;

                            newPasswordRepeatField.txtLimitedInput.Background = Brushes.Transparent;
                            newPasswordRepeatField.txtLimitedInput.BorderBrush = Brushes.Transparent;

                            IsValid = true;
                        }
                        #endregion

                        #region Валидация нового пароля
                        string passwordError = ChatUser.PasswordValidation(newP);
                        if (passwordError != null)
                        {
                            newPasswordField.txtLimitedInput.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.2 };
                            newPasswordField.txtLimitedInput.BorderBrush = new SolidColorBrush(Colors.Red);
                            newPasswordField.txtLimitedInput.ToolTip = CreateToolTip(passwordError);
                            IsValid = false;
                        }
                        else
                        {
                            newPasswordField.txtLimitedInput.Background = Brushes.Transparent;
                            newPasswordField.txtLimitedInput.BorderBrush = Brushes.Transparent;
                            newPasswordField.txtLimitedInput.ToolTip = null;
                        }
                        #endregion

                        if (IsValid)
                        {
                            db.Database.ExecuteSqlCommand("update ChatUser set Password='" +
                                newPasswordField.txtLimitedInput.Text + "' where Id=" + user.Id);

                            db.SaveChanges();

                            // Popup о том, что данные сохранены
                            SavePasswordPopup.IsOpen = true;
                            // таймер на показ Popup на 2 секунды
                            DispatcherTimer time = new DispatcherTimer();
                            time.Interval = TimeSpan.FromSeconds(2);
                            time.Start();
                            time.Tick += delegate
                            {
                                SavePasswordPopup.IsOpen = false;
                                time.Stop();
                            };

                            oldPasswordField.txtLimitedInput.Text = "";
                            oldPasswordField.txtLimitedInput.Background = Brushes.Transparent;
                            oldPasswordField.txtLimitedInput.BorderBrush = Brushes.Transparent;

                            newPasswordField.txtLimitedInput.Text = "";
                            newPasswordField.txtLimitedInput.Background = Brushes.Transparent;
                            newPasswordField.txtLimitedInput.BorderBrush = Brushes.Transparent;

                            newPasswordRepeatField.txtLimitedInput.Text = "";
                            newPasswordRepeatField.txtLimitedInput.Background = Brushes.Transparent;
                            newPasswordRepeatField.txtLimitedInput.BorderBrush = Brushes.Transparent;

                            changePasswordGrid.Visibility = Visibility.Collapsed;
                            changePasswordTextBlock.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            // Popup о том, что надо исправить ошибки
                            ErrorPasswordPopup.IsOpen = true;
                            // таймер на показ Popup на 2 секунды
                            DispatcherTimer time = new DispatcherTimer();
                            time.Interval = TimeSpan.FromSeconds(2);
                            time.Start();
                            time.Tick += delegate
                            {
                                ErrorPasswordPopup.IsOpen = false;
                                time.Stop();
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("При попытке сохранить данные произошла ошибка.\nТекст ошибки:\t" + ex.Message);
                    return;
                }
            }
            else
            {
                // Popup о том, что надо исправить ошибки
                ErrorPasswordPopup.IsOpen = true;
                // таймер на показ Popup на 2 секунды
                DispatcherTimer time = new DispatcherTimer();
                time.Interval = TimeSpan.FromSeconds(2);
                time.Start();
                time.Tick += delegate
                {
                    ErrorPasswordPopup.IsOpen = false;
                    time.Stop();
                };
            }
        }
    }
}
