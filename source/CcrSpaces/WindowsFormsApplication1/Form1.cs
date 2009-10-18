using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Ccr.Core;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private readonly EventStream mouseMovements = new EventStream();

        public Form1()
        {
            InitializeComponent();

            this.MouseMove += this.mouseMovements;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
//new[] {1, 2, 3, 4, 5, 6, 7}
//    .Where(n => n%2 != 0)
//    .Transform(n => n*n)
//    .Transform(n => -n)
//    .Subscribe(Console.WriteLine);

            this.mouseMovements.Events<MouseEventArgs>()
                .Where(args => args.X < 100 && args.Y < 100)
                .Subscribe(args => Console.WriteLine("{0}, {1}", args.X, args.Y));
        }


    }


    static class IEnumExt
    {
        public static void Subscribe<T>(this IEnumerable<T> eventStream, Action<T> handleEvent)
        {
            foreach (T e in eventStream)
                handleEvent(e);
        }

        public static IEnumerable<TOut> Transform<TIn, TOut>(this IEnumerable<TIn> eventStream, Func<TIn, TOut> processEvent)
        {
            foreach (TIn e in eventStream)
                yield return processEvent(e);
        }
    }


class EventStream
{
    public class Event
    {
        public object Sender;
        public EventArgs Args;
    }

    private Port<Event> events = new Port<Event>();


    private void EventListener(object sender, EventArgs args)
    {
        this.events.Post(new Event{Sender = sender, Args=args});
    }


    public IEnumerable<TEvent> Events<TEvent>() where TEvent : EventArgs
    {
        Event e;
        while (this.events.Test(out e))
            yield return (TEvent)e.Args;
    }


    public static implicit operator MouseEventHandler(EventStream stream)
    {
        return stream.EventListener;
    }
}
}
