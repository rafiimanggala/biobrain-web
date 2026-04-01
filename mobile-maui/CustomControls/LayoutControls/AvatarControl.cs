using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using BioBrain.AppResources;
using IOPath = System.IO.Path;

namespace CustomControls.LayoutControls
{
    /// <summary>
    /// Avatar selection control that displays a circular avatar image with an edit overlay.
    /// Tap to pick a photo from the device gallery.
    /// Converted from Xamarin.Forms (CircleImage + Plugin.Media) to .NET MAUI
    /// (Border with EllipseGeometry + MediaPicker).
    /// </summary>
    public class AvatarControl : ContentView
    {
        private readonly Image _avatarImage;
        private readonly Border _circleBorder;
        private readonly Image _addButtonImage;

        private const string DefaultSelectAvatarString = "EDIT";

        private IFilesPath Paths
        {
            get
            {
#if ANDROID || IOS
                return DependencyService.Get<IFilesPath>();
#else
                return null;
#endif
            }
        }

        public AvatarControl()
        {
            _avatarImage = new Image
            {
                Aspect = Aspect.AspectFill,
                WidthRequest = 120,
                HeightRequest = 120,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };

            _circleBorder = new Border
            {
                StrokeShape = new Ellipse(),
                StrokeThickness = 0,
                Stroke = Colors.Transparent,
                BackgroundColor = Colors.White,
                WidthRequest = 120,
                HeightRequest = 120,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10),
                Content = _avatarImage,
            };

            var circleTapGesture = new TapGestureRecognizer();
            circleTapGesture.Tapped += OnTapped;
            _circleBorder.GestureRecognizers.Add(circleTapGesture);

            _addButtonImage = new Image
            {
                Source = "addbutton.png",
                HeightRequest = 30,
                WidthRequest = 30,
                Margin = new Thickness(0, 0, 40, 10),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
            };

            var addTapGesture = new TapGestureRecognizer();
            addTapGesture.Tapped += OnTapped;
            _addButtonImage.GestureRecognizers.Add(addTapGesture);

            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 180,
            };

            grid.Children.Add(_circleBorder);
            grid.Children.Add(_addButtonImage);

            Content = grid;

            AvatarPath = string.Empty;
        }

        #region BindableProperties

        public static readonly BindableProperty AvatarPathProperty =
            BindableProperty.Create(
                nameof(AvatarPath),
                typeof(string),
                typeof(AvatarControl),
                string.Empty,
                BindingMode.TwoWay,
                propertyChanged: OnAvatarPathChanged);

        public string AvatarPath
        {
            get => (string)GetValue(AvatarPathProperty);
            set => SetValue(AvatarPathProperty, value);
        }

        private static void OnAvatarPathChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = (AvatarControl)bindable;
            var value = (string)newValue;
            ctrl.ApplyAvatarPath(value);
        }

        private void ApplyAvatarPath(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                IsAvatarSelected = false;
                _avatarImage.Source = ImageSource.FromFile(DefaultAvatar);
            }
            else
            {
                IsAvatarSelected = true;
                if (value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    _avatarImage.Source = ImageSource.FromUri(new Uri(value));
                }
                else
                {
                    var avatarDir = Paths?.AvatarPath;
                    _avatarImage.Source = !string.IsNullOrEmpty(avatarDir)
                        ? ImageSource.FromFile(IOPath.Combine(avatarDir, value))
                        : ImageSource.FromFile(value);
                }
            }
        }

        public static readonly BindableProperty DefaultAvatarProperty =
            BindableProperty.Create(
                nameof(DefaultAvatar),
                typeof(string),
                typeof(AvatarControl),
                "DefaultAvatar.png",
                BindingMode.TwoWay,
                propertyChanged: OnDefaultAvatarChanged);

        public string DefaultAvatar
        {
            get => (string)GetValue(DefaultAvatarProperty);
            set => SetValue(DefaultAvatarProperty, value);
        }

        private static void OnDefaultAvatarChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = (AvatarControl)bindable;
            if (string.IsNullOrEmpty(ctrl.AvatarPath))
            {
                ctrl.IsAvatarSelected = false;
                ctrl._avatarImage.Source = ImageSource.FromFile((string)newValue);
            }
        }

        public static new readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(
                "BorderColor",
                typeof(Color),
                typeof(AvatarControl),
                Colors.Transparent,
                BindingMode.TwoWay,
                propertyChanged: OnBorderColorChanged);

        public new Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        private static void OnBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = (AvatarControl)bindable;
            ctrl._circleBorder.Stroke = new SolidColorBrush((Color)newValue);
        }

        public static readonly BindableProperty BorderThicknessProperty =
            BindableProperty.Create(
                nameof(BorderThickness),
                typeof(double),
                typeof(AvatarControl),
                0.0,
                BindingMode.TwoWay,
                propertyChanged: OnBorderThicknessChanged);

        public double BorderThickness
        {
            get => (double)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        private static void OnBorderThicknessChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = (AvatarControl)bindable;
            ctrl._circleBorder.StrokeThickness = (double)newValue;
        }

        #endregion

        #region Events

        public event EventHandler<SelectedEventArgs> AvatarSelected;
        public event EventHandler AvatarSelecting;
        public event EventHandler AvatarDeleted;

        #endregion

        #region Public State

        public bool IsAvatarSelected { get; private set; }

        #endregion

        #region Event Handlers

        private void OnTapped(object sender, EventArgs e)
        {
            OnAvatarSelecting();
        }

        protected virtual async void OnAvatarSelecting()
        {
            AvatarSelecting?.Invoke(this, EventArgs.Empty);

            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select photo"
                });

                if (result == null)
                    return;

                var sourcePath = result.FullPath;
                if (string.IsNullOrEmpty(sourcePath))
                    return;

                // Move file to app directory if IWorkingWithFiles is available
                var fileWorker = DependencyService.Get<IWorkingWithFiles>();
                if (fileWorker != null)
                {
                    sourcePath = fileWorker.MoveFileToAppDirectory(sourcePath);
                }

                // Crop to square if IImageWorker is available
                var imageWorker = DependencyService.Get<Common.Interfaces.IImageWorker>();
                if (imageWorker != null && !imageWorker.CutToSquare(sourcePath))
                {
                    OnAvatarSelectedInternal(string.Empty, "Wrong file format");
                    return;
                }

                var fileName = IOPath.GetFileName(sourcePath);

                if (!string.IsNullOrEmpty(AvatarPath))
                {
                    AvatarPath = fileName;
                }

                OnAvatarSelectedInternal(fileName);
            }
            catch (PermissionException)
            {
                OnAvatarSelectedInternal(string.Empty, "Photo permission denied");
            }
            catch (Exception ex)
            {
                OnAvatarSelectedInternal(string.Empty, ex.Message);
            }
        }

        /// <summary>
        /// Fires the AvatarSelected event. Public so platform-specific code can invoke it.
        /// Matches the original Xamarin public method signature.
        /// </summary>
        public virtual void OnAvatarSelected(string path, string error = "")
        {
            OnAvatarSelectedInternal(path, error);
        }

        private void OnAvatarSelectedInternal(string path, string error = "")
        {
            AvatarSelected?.Invoke(this, new SelectedEventArgs { Path = path, Error = error });
        }

        protected virtual void OnAvatarDeleted()
        {
            AvatarDeleted?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
