// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Trumpf.Coparoo.Web.Waiting
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Exceptions;

    /// <summary>
    /// Dialog wait class.
    /// </summary>
    public class DialogWait
    {
        private object m = new object();
        private State state;
        private TimeSpan gto;
        private TimeSpan positiveTimeout;
        private TimeSpan bto;
        private TimeSpan negativeTimeout;
        private DialogView uic;
        private static readonly TimeSpan timerPeriod = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan negativeWaitTime = TimeSpan.FromSeconds(20);
        private static readonly TimeSpan positiveWaitTime = TimeSpan.FromSeconds(0);
        private static readonly TimeSpan positiveWaitTimeWithAction = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogWait"/> class.
        /// </summary>
        private DialogWait()
        {
        }

        /// <summary>
        /// Windows enum for enabling click-through.
        /// </summary>
        private enum GWL
        {
            /// <summary>
            /// A value.
            /// </summary>
            ExStyle = -20
        }

        /// <summary>
        /// Dialog states.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// initial state.
            /// </summary>
            init,

            /// <summary>
            /// unknown state.
            /// </summary>
            unknown,

            /// <summary>
            /// good state.
            /// </summary>
            good,

            /// <summary>
            /// bad state.
            /// </summary>
            bad,

            /// <summary>
            /// good timeout state.
            /// </summary>
            good_timedout,

            /// <summary>
            /// good user exit state.
            /// </summary>
            good_userexit,

            /// <summary>
            /// bad timeout state.
            /// </summary>
            bad_timedout,

            /// <summary>
            /// bad user exit state.
            /// </summary>
            bad_userexit
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// Shows a dialog.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        public static void For(Func<bool> function, string expectationText)
        {
            For(function, expectationText, negativeWaitTime);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// Shows a dialog.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="condition">The condition to evaluate on the functions return value.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        public static void For<T>(Func<T> function, Predicate<T> condition, string expectationText)
        {
            For<T>(function, condition, expectationText, negativeWaitTime);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// Shows a dialog.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        /// <param name="timeout">The timeout.</param>
        public static void For(Func<bool> function, string expectationText, TimeSpan timeout)
        {
            For(function, expectationText, timeout, positiveWaitTime, timerPeriod, false);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// Shows a dialog.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="condition">The condition to evaluate on the functions return value.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        /// <param name="timeout">The timeout.</param>
        public static void For<T>(Func<T> function, Predicate<T> condition, string expectationText, TimeSpan timeout)
        {
            For(function, condition, expectationText, timeout, positiveWaitTime, timerPeriod, false, null);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// Shows a dialog.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        /// <param name="negativeTimeout">The negative timeout.</param>
        /// <param name="positiveTimeout">The positive timeout.</param>
        /// <param name="pollingPeriod">The polling time.</param>
        public static void For(Func<bool> function, string expectationText, TimeSpan negativeTimeout, TimeSpan positiveTimeout, TimeSpan pollingPeriod)
        {
            For(function, expectationText, negativeTimeout, positiveTimeout, pollingPeriod, false);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// Shows a dialog.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="condition">The condition to evaluate on the functions return value.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        /// <param name="negativeTimeout">The negative timeout.</param>
        /// <param name="positiveTimeout">The positive timeout.</param>
        /// <param name="pollingPeriod">The polling time.</param>
        public static void For<T>(Func<T> function, Predicate<T> condition, string expectationText, TimeSpan negativeTimeout, TimeSpan positiveTimeout, TimeSpan pollingPeriod)
        {
            For(function, condition, expectationText, negativeTimeout, positiveTimeout, pollingPeriod, false, null);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// Shows a dialog.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="condition">The condition to evaluate on the functions return value.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        /// <param name="negativeTimeout">The negative timeout.</param>
        /// <param name="positiveTimeout">The positive timeout.</param>
        public static void For<T>(Func<T> function, Predicate<T> condition, string expectationText, TimeSpan negativeTimeout, TimeSpan positiveTimeout)
        {
            For(function, condition, expectationText, negativeTimeout, positiveTimeout, timerPeriod, false, null);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>. Shows an action text.
        /// </summary>
        /// <param name="actionText">The action to display.</param>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="condition">The condition to evaluate on the functions return value.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        /// <param name="negativeTimeout">The negative timeout.</param>
        /// <param name="positiveTimeout">The positive timeout.</param>
        public static void ForAction<T>(string actionText, Func<T> function, Predicate<T> condition, string expectationText, TimeSpan negativeTimeout, TimeSpan positiveTimeout)
        {
            For(function, condition, expectationText, negativeTimeout, positiveTimeout, timerPeriod, false, actionText);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>. Shows an action text.
        /// </summary>
        /// <param name="actionText">The action to display.</param>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="condition">The condition to evaluate on the functions return value.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        public static void ForAction<T>(string actionText, Func<T> function, Predicate<T> condition, string expectationText)
        {
            For(function, condition, expectationText, TimeSpan.MaxValue, positiveWaitTimeWithAction, timerPeriod, false, actionText);
        }

        /// <summary>
        /// Shows the action and expectation text. Requires manual acknowledgment.
        /// </summary>
        /// <param name="actionText">The action to display.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        public static void ForAction(string actionText, string expectationText)
        {
            For<object>(null, null, expectationText, TimeSpan.MaxValue, TimeSpan.Zero, timerPeriod, false, actionText);
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// Shows a dialog.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        /// <param name="negativeTimeout">The negative timeout.</param>
        /// <param name="positiveTimeout">The positive timeout.</param>
        /// <param name="pollingPeriod">The polling time.</param>
        /// <param name="clickThrough">Whether to enable click-through mode.</param>
        public static void For(Func<bool> function, string expectationText, TimeSpan negativeTimeout, TimeSpan positiveTimeout, TimeSpan pollingPeriod, bool clickThrough)
        {
            if (function == null)
            {
                For<object>(null, null, expectationText, negativeTimeout, positiveTimeout, pollingPeriod, clickThrough, null);
            }
            else
            {
                For<object>(null, _ => function(), expectationText, negativeTimeout, positiveTimeout, pollingPeriod, clickThrough, null);
            }
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// Shows a dialog including the current value.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="condition">The condition to evaluate on the functions return value.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        /// <param name="negativeTimeout">The negative timeout.</param>
        /// <param name="positiveTimeout">The positive timeout.</param>
        /// <param name="pollingPeriod">The polling time.</param>
        /// <param name="clickThrough">Whether to enable click-through mode.</param>
        /// <param name="actionText">The action text.</param>
        public static void For<T>(Func<T> function, Predicate<T> condition, string expectationText, TimeSpan negativeTimeout, TimeSpan positiveTimeout, TimeSpan pollingPeriod, bool clickThrough, string actionText)
        {
            new DialogWait().Forr(function, condition, expectationText, negativeTimeout, positiveTimeout, pollingPeriod, clickThrough, actionText);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr wnd, GWL index);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(IntPtr wnd, GWL index, int newLong);

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr wnd, int msg, int param1, int param2);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int wmsg, bool wparam, int lparam);

        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        /// <summary>
        /// On dialog load.
        /// </summary>
        /// <param name="expectationText">Text describing the expected condition.</param>
        /// <param name="actionText">The action text.</param>
        private void OnDialogLoad(string expectationText, string actionText)
        {
            lock (m)
            {
                switch (state)
                {
                    case State.init:
                        uic.ExpectationText = expectationText;
                        if (actionText != null)
                        {
                            uic.ActionText = actionText;
                        }

                        state = State.unknown;
                        gto = positiveTimeout;
                        bto = negativeTimeout;
                        uic.GoodButtonEnabled = null;
                        uic.AutoActionGoodText = gto;
                        uic.AutoActionBadText = bto;
                        uic.Value = "unknown";
                        uic.Show();
                        break;

                    default: throw new InvalidOperationException(state.ToString());
                }
            }
        }

        /// <summary>
        /// Enter exit bad state.
        /// </summary>
        private void EnterExitBad()
        {
            uic.Close();
            state = bto <= TimeSpan.Zero ? State.bad_timedout : State.bad_userexit;
        }

        /// <summary>
        /// Enter exit bad state.
        /// </summary>
        private void OnBadClick()
        {
            lock (m)
            {
                switch (state)
                {
                    case State.unknown:
                    case State.good:
                    case State.bad:
                        EnterExitBad();
                        break;

                    case State.bad_timedout:
                    case State.bad_userexit:
                    case State.good_timedout:
                    case State.good_userexit:
                        // ignore badClick
                        break;

                    default: throw new InvalidOperationException(state.ToString());
                }
            }
        }

        /// <summary>
        /// Enter exit good state.
        /// </summary>
        private void EnterExitGood()
        {
            uic.Close();
            state = gto <= TimeSpan.Zero ? State.good_timedout : State.good_userexit;
        }

        /// <summary>
        /// Enter exit good state.
        /// </summary>
        private void OnGoodClick()
        {
            lock (m)
            {
                switch (state)
                {
                    case State.good:
                    case State.unknown:
                        EnterExitGood();
                        break;

                    case State.bad_timedout:
                    case State.bad_userexit:
                    case State.good_timedout:
                    case State.good_userexit:
                        // ignore goodClick
                        break;

                    default: throw new InvalidOperationException(state.ToString());
                }
            }
        }

        /// <summary>
        /// On value changed.
        /// </summary>
        /// <param name="value">The value.</param>
        private void OnValueChanged(string value)
        {
            SpinWait.SpinUntil(() => state != State.init);

            lock (m)
            {
                switch (state)
                {
                    case State.unknown:
                    case State.good:
                    case State.bad:
                        uic.Value = value;
                        break;

                    case State.bad_timedout:
                    case State.bad_userexit:
                    case State.good_timedout:
                    case State.good_userexit:
                        // ignore update
                        break;

                    default: throw new InvalidOperationException(state.ToString());
                }
            }
        }

        /// <summary>
        /// On truth changed.
        /// </summary>
        /// <param name="truth">The truth value.</param>
        private void OnTruthChanged(bool truth)
        {
            SpinWait.SpinUntil(() => state != State.init);

            lock (m)
            {
                var lastState = state;

                switch (state)
                {
                    case State.unknown:
                    case State.good:
                    case State.bad:
                        uic.SuspendDrawing();
                        uic.GoodButtonEnabled = truth;
                        state = truth ? State.good : State.bad;
                        gto = lastState == State.bad && state == State.good ? positiveTimeout : gto;
                        uic.ResumeDrawing();
                        break;

                    case State.bad_timedout:
                    case State.bad_userexit:
                    case State.good_timedout:
                    case State.good_userexit:
                        // ignore update
                        break;

                    default: throw new InvalidOperationException(state.ToString());
                }
            }
        }

        /// <summary>
        /// On timer elapsed.
        /// </summary>
        private void OnTimerElapsed()
        {
            lock (m)
            {
                switch (state)
                {
                    case State.unknown:
                    case State.bad:
                        bto -= bto == TimeSpan.MaxValue ? TimeSpan.Zero : timerPeriod;
                        if (bto <= TimeSpan.Zero)
                        {
                            EnterExitBad();
                        }
                        else
                        {
                            uic.AutoActionBadText = bto;
                        }
                        break;

                    case State.good:
                        gto -= gto == TimeSpan.MaxValue ? TimeSpan.Zero : timerPeriod;
                        if (gto <= TimeSpan.Zero)
                        {
                            EnterExitGood();
                        }
                        else
                        {
                            uic.AutoActionGoodText = gto;
                        }
                        break;

                    case State.bad_timedout:
                    case State.bad_userexit:
                    case State.good_timedout:
                    case State.good_userexit:
                        // ignore update
                        break;

                    default: throw new InvalidOperationException(state.ToString());
                }
            }
        }

        /// <summary>
        /// Waits until a function evaluates to <c>true</c>.
        /// Shows a dialog.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="condition">The condition to evaluate on the functions return value.</param>
        /// <param name="expectationText">Text that explains the function's expectation.</param>
        /// <param name="negativeTimeout">The negative timeout.</param>
        /// <param name="positiveTimeout">The positive timeout.</param>
        /// <param name="pollingPeriod">The polling time.</param>
        /// <param name="clickThrough">Whether to enable click-through mode.</param>
        /// <param name="actionText">The action text.</param>
        private void Forr<T>(Func<T> function, Predicate<T> condition, string expectationText, TimeSpan negativeTimeout, TimeSpan positiveTimeout, TimeSpan pollingPeriod, bool clickThrough, string actionText)
        {
            // init
            state = State.init;
            this.positiveTimeout = positiveTimeout;
            this.negativeTimeout = negativeTimeout;
            uic = new DialogView(negativeTimeout != TimeSpan.MaxValue, positiveTimeout != TimeSpan.MaxValue && positiveTimeout != TimeSpan.Zero, clickThrough, function != null, actionText);

            // spawn
            var c = new CancellationTokenSource();
            Task ui = new Task(() => uic.UI(() => OnDialogLoad(expectationText, actionText), OnBadClick, OnGoodClick));
            Task ti = new Task(() => Timer(c.Token));
            Task po = new Task(() => Evaluator(c.Token, function, condition, pollingPeriod));

            // join
            ui.Start();
            ti.Start();
            po.Start();
            ui.Wait();

            c.Cancel();
            ti.Wait();
            po.Wait();

            switch (state)
            {
                case State.good_userexit:
                case State.good_timedout:
                    return;

                case State.bad_timedout:
                    throw new DialogWaitForTimeoutException(expectationText, negativeTimeout);

                case State.bad_userexit:
                    throw new DialogWaitForAbortedException(expectationText);

                default: throw new InvalidOperationException(state.ToString());
            }
        }

        /// <summary>
        /// Timer function.
        /// </summary>
        /// <param name="c">The cancellation token.</param>
        private void Timer(CancellationToken c)
        {
            SpinWait.SpinUntil(() => state != State.init);

            while (!c.IsCancellationRequested)
            {
                OnTimerElapsed();

                if (c.IsCancellationRequested)
                {
                    break;
                }

                Thread.Sleep(timerPeriod);
            }
        }

        /// <summary>
        /// Evaluator function.
        /// </summary>
        /// <param name="c">The cancellation token.</param>
        /// <param name="function">The function to call periodically.</param>
        /// <param name="condition">The condition to evaluate on the functions return value.</param>
        /// <param name="pollingPeriod">The polling time.</param>
        private void Evaluator<T>(CancellationToken c, Func<T> function, Predicate<T> condition, TimeSpan pollingPeriod)
        {
            var stopwatch = new Stopwatch();

            bool first = true;
            T lastValue = default(T);
            T value = default(T);
            bool lastTruth = default(bool);
            bool truth = default(bool);
            while (condition != null && !c.IsCancellationRequested)
            {
                stopwatch.Restart();

                if (function != null)
                {
                    value = function();
                    if (first || !value.Equals(lastValue))
                    {
                        OnValueChanged(value.ToString());
                        lastValue = value;
                    }
                }

                truth = condition(value);
                if (first || !truth.Equals(lastTruth))
                {
                    OnTruthChanged(truth);
                    lastTruth = truth;
                }

                var remaining = pollingPeriod - stopwatch.Elapsed;
                Thread.Sleep(remaining >= TimeSpan.Zero && !c.IsCancellationRequested ? remaining : TimeSpan.Zero);
                first = false;
            }
        }

        /// <summary>
        /// The view class.
        /// </summary>
        private class DialogView
        {
            // create and add dialog components
            private Label expectedTextLabel;
            private Label actionTextLabel;
            private Label valueLabel;
            private Label autoActionGoodLabel;
            private Label autoActionBadLabel;
            private Button positiveButton;
            private Button negativeButton;
            private Form dialog;
            private bool clickThrough;
            private bool showCurrentValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="DialogView"/> class.
            /// </summary>
            /// <param name="showAutoActionBadLabel">Whether to show the auto action bad label.</param>
            /// <param name="showAutoActionGoodLabel">Whether to show the auto action good label.</param>
            /// <param name="clickThrough">Whether to enable click-through mode.</param>
            /// <param name="showCurrentValue">Whether to show the current value.</param>
            /// <param name="actionText">The action text.</param>
            public DialogView(bool showAutoActionBadLabel, bool showAutoActionGoodLabel, bool clickThrough, bool showCurrentValue, string actionText)
            {
                this.clickThrough = clickThrough;
                this.showCurrentValue = showCurrentValue;

                const int WIDTH = 500;
                const int SPACE = 30;
                const int ITOP = 20;
                const int LEFT = 30;
                int top = ITOP;

                var font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);

                // dialog
                dialog = new Form() { Width = LEFT + WIDTH, FormBorderStyle = FormBorderStyle.None, ShowInTaskbar = false, Opacity = 0.8, StartPosition = FormStartPosition.Manual, Visible = false, TopMost = false };

                // expected text
                dialog.Controls.Add(expectedTextLabel = new Label() { Left = LEFT, Top = top, Width = WIDTH, Font = font });

                // prepare label for two text lines.
                top += expectedTextLabel.Height;
                expectedTextLabel.Height = 2 * expectedTextLabel.Height;

                if (actionText != null)
                {
                    dialog.Controls.Add(actionTextLabel = new Label() { Left = LEFT, Top = top += SPACE, Width = WIDTH, Font = font });
                }

                if (showCurrentValue)
                {
                    dialog.Controls.Add(valueLabel = new Label() { Left = LEFT, Top = top += SPACE, Width = WIDTH, Font = font });
                }

                // good timeout label
                if (showAutoActionGoodLabel)
                {
                    dialog.Controls.Add(autoActionGoodLabel = new Label() { Left = LEFT, Top = top += SPACE, Width = WIDTH, Font = font });
                }

                // bad timeout label
                if (showAutoActionBadLabel)
                {
                    dialog.Controls.Add(autoActionBadLabel = new Label() { Left = LEFT, Top = top += SPACE, Width = WIDTH, Font = font });
                }

                // positive button
                if (!clickThrough)
                {
                    var dialogWidth = dialog.Width;

                    dialog.Controls.Add(positiveButton = new Button() { Left = dialogWidth / 2, Height = 2 * SPACE, Width = dialogWidth / 2, Top = top += SPACE, DialogResult = DialogResult.OK, Text = "Positive", FlatStyle = FlatStyle.Flat, Font = font });

                    // negative button
                    dialog.Controls.Add(negativeButton = new Button() { Left = 0, Height = 2 * SPACE, Width = dialogWidth / 2, Top = top, DialogResult = DialogResult.Cancel, Text = "Negative", FlatStyle = FlatStyle.Flat, BackColor = Color.Red, Font = font });

                    top += SPACE;
                }

                // set dialog size
                dialog.Height = top + ITOP + SPACE;
                dialog.Location = new Point(Screen.PrimaryScreen.Bounds.Width - dialog.Width, 0);
            }

            /// <summary>
            /// Sets the auto action good text.
            /// </summary>
            public TimeSpan AutoActionGoodText
            {
                set { Invoke(() => autoActionGoodLabel.Text = "Continue in " + value.TotalSeconds.ToString("0.0") + " seconds", autoActionGoodLabel != null); }
            }

            /// <summary>
            /// Sets the auto action bad text.
            /// </summary>
            public TimeSpan AutoActionBadText
            {
                set { Invoke(() => autoActionBadLabel.Text = "Abort in " + value.TotalSeconds.ToString("0.0") + " seconds", autoActionBadLabel != null); }
            }

            /// <summary>
            /// Sets the expected text.
            /// </summary>
            public string ExpectationText
            {
                set { Invoke(() => expectedTextLabel.Text = "Expectation: " + value); }
            }

            /// <summary>
            /// Sets the action text.
            /// </summary>
            public string ActionText
            {
                set { Invoke(() => actionTextLabel.Text = "Action: " + value); }
            }

            /// <summary>
            /// Sets the good button text.
            /// </summary>
            public string GoodButtonText
            {
                set { Invoke(() => positiveButton.Text = value); }
            }

            /// <summary>
            /// Sets the bad button text.
            /// </summary>
            public string BadButtonText
            {
                set { Invoke(() => negativeButton.Text = value); }
            }

            /// <summary>
            /// Sets the current value.
            /// </summary>
            public string Value
            {
                set { Invoke(() => valueLabel.Text = "Observed value: " + value, showCurrentValue); }
            }

            /// <summary>
            /// Sets a value indicating whether the good button is enabled.
            /// </summary>
            public bool? GoodButtonEnabled
            {
                set
                {
                    Invoke(() =>
                    {
                        if (positiveButton != null)
                        {
                            positiveButton.Enabled = !(value == false);
                        }

                        dialog.BackColor = !value.HasValue ? Color.Gray : (value == true ? Color.Green : Color.Red);
                        dialog.Invalidate();
                    });
                }
            }

            /// <summary>
            /// Invoke an action.
            /// </summary>
            /// <param name="a">action.</param>
            /// <param name="condition">Whether to execute the action.</param>
            private void Invoke(Action a, bool condition = true)
            {
                if (condition)
                {
                    if (dialog.InvokeRequired)
                    {
                        dialog.BeginInvoke((MethodInvoker)(() => a()));
                    }
                    else
                    {
                        a();
                    }
                }
            }

            /// <summary>
            /// Show the dialog.
            /// </summary>
            /// <param name="dialogLoad">The dialog load action.</param>
            /// <param name="badClick">The bad click action.</param>
            /// <param name="goodClick">The good click action.</param>
            public void UI(Action dialogLoad, Action badClick, Action goodClick)
            {
                dialog.Load += (s, o) => dialogLoad();
                if (negativeButton != null)
                {
                    negativeButton.Click += (s, o) => badClick();
                }

                if (positiveButton != null)
                {
                    positiveButton.Click += (s, o) => goodClick();
                }

                if (!clickThrough)
                {
                    dialog.MouseMove += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            ReleaseCapture();
                            SendMessage(dialog.Handle, 0xA1, 0x2, 0);
                        }
                    };
                }

                dialog.ShowDialog();
            }

            /// <summary>
            /// Close the dialog.
            /// </summary>
            public void Close()
            {
                Invoke(() => dialog.Close());
            }

            /// <summary>
            /// Sets a value indicating whether the view is visible.
            /// </summary>
            public void Show()
            {
                Invoke(() =>
                {
                    dialog.Visible = true;
                    dialog.BringToFront();
                    dialog.TopMost = true;
                    dialog.TopLevel = true;
                    if (clickThrough)
                    {
                        MakeClickThrough();
                    }
                });
            }

            /// <summary>
            /// Suspend drawing.
            /// </summary>
            public void SuspendDrawing()
            {
                Invoke(() => SendMessage(dialog.Handle, 11, false, 0));
            }

            /// <summary>
            /// Resume drawing.
            /// </summary>
            public void ResumeDrawing()
            {
                Invoke(() =>
                {
                    SendMessage(dialog.Handle, 11, true, 0);
                    dialog.Refresh();
                });
            }

            /// <summary>
            /// Make dialog click-through.
            /// </summary>
            private void MakeClickThrough()
            {
                int wl = GetWindowLong(dialog.Handle, GWL.ExStyle);
                wl = wl | 0x80000 | 0x20;
                SetWindowLong(dialog.Handle, GWL.ExStyle, wl);
            }
        }
    }
}