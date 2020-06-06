using System;
using System.Collections.Generic;
using System.Diagnostics; //temporary to help with debbuging`   

namespace WeebQuizer_server_remote
{
    // -------------------------- ENUMS -------------------------------------
    public enum horizontal_aligment
    {
        left, center, right
    }
    public enum vertical_aligment
    {
        up, center, down
    }
    // Think about separate file for enums!
    // ----------------------------------------------------------------------

    
    /// <summary>
    /// Class to provide control over functions related to sreen size and content displaying.
    /// </summary>
    class Screen_Manager
    {
        // ---------------------- VARIABLES --------------------------------
        //private variables which storage console dimensions
        private int screen_height, screen_width;
        //Queues to provide informations about text position in the screen
        //These queues are used in 'text functions'
        private Queue<string> text_buffer = new Queue<string>(); //test
        private Queue<horizontal_aligment> horizontal_text_position_buffer = new Queue<horizontal_aligment>();
        private Queue<int> text_x_position = new Queue<int>();
        private Queue<int> text_y_position = new Queue<int>();
        // TO DO: This comment
        private vertical_aligment last_text_position;
        int last_line = 0;
        // -------------------------------------------------------------------

        // --------------------- CONSTRUCTORS --------------------------------
        /// <summary>
        /// Default constructor. Console size will be same as usual.
        /// </summary>
        public Screen_Manager()
        {
            //Setting local variables
            this.screen_height = Console.BufferHeight = Console.WindowHeight;
            this.screen_width = Console.BufferWidth = Console.WindowWidth;
        }

        /// <summary>
        /// Constructor which allows you to set custom console size.
        /// </summary>
        /// <param name="height">Sets console height</param>
        /// <param name="width">Sets console width</param>
        public Screen_Manager(int height, int width)
        {
            //Setting local variables 
            this.screen_height = height;
            this.screen_width = width;
            //Setting console size
            Console.WindowHeight =  Console.BufferHeight = height;
            Console.WindowWidth = Console.BufferWidth = width;
        }
        // --------------------------------------------------------------------

        /// <summary>
        /// Same effect as 'Console.CursorVisible' (honestly it's exactly what it is) but now it's part of 'ScreenManager' class, so... 
        /// </summary>
        /// <param name="active"></param>
        public void CursorVisiblity(bool active)
        {
            Console.CursorVisible = active;
        }

        public void Clear()
        {
            Console.Clear();
            this.last_line = 0;
        }


        // --------------------------------- TEXT FUNCTIONS: ------------------------------------
        //These functions are used to simplify text output in console aplication
                
        /// <summary>
        /// Internal function that is responsible of printing line of text with aligment.
        /// </summary>
        /// <param name="text">Text to write to the console.</param>
        /// <param name="horizontal_pos">Horizontal aligment.</param>
        /// <param name="vertical_pos">Vertical aligment.</param>
        /// <param name="line_count">Number of lines to print.</param>
        /// <param name="line_number">Number of printed line.</param>
        private void WriteAtLine(string text, horizontal_aligment horizontal_pos, vertical_aligment vertical_pos, int line_count, int line_number) 
        {
            int x_pos = 0;
            int y_pos = 0;

            //Check if line number or text lngth aren't bigger than maximum height and width of console.
            if (this.last_line > this.screen_height || text.Length > this.screen_width)
            {
                //TO DO
            }

            switch (vertical_pos){
                case vertical_aligment.up:
                    y_pos = this.last_line;
                    last_line = y_pos+1;
                    break;
                case vertical_aligment.center:
                    y_pos = ((((this.screen_height / 2) - (line_count / 2) + line_number)+last_line/2)-1);

                    Debug.WriteLine("Line number: " + line_number);
                    Debug.WriteLine("Line count: " + line_count);
                    if(line_number.Equals(line_count-1))
                    {
                        this.last_line = (((this.screen_height / 2) - (line_count / 2) + line_number +1)+last_line/2);
                        Debug.WriteLine("Last line: " + this.last_line);
                    }
                    break;
                case vertical_aligment.down:
                    y_pos = (this.screen_height - (line_count+1)) + line_number;
                    last_line = this.screen_height - 1;
                    break;
            }

            switch (horizontal_pos){
                case horizontal_aligment.left:
                    x_pos = 0;
                    break;
                case horizontal_aligment.center:
                    x_pos = ((this.screen_width / 2) - (text.Length / 2)); 
                    break;
                case horizontal_aligment.right:
                    x_pos = this.screen_width - (text.Length+1);
                    break;
            }

            // TO DO: Update 'last position'
            
            Console.SetCursorPosition(x_pos, y_pos);
            Console.WriteLine(text);
        }
        
        /// <summary>
        /// Method basd on 'Console.WriteLine()'. In its basic form you can just write normal line of text.
        /// Aligned to up and left, write from left to right
        /// </summary>
        /// <param name="text">Text to write to the console.</param>
        public void WriteLine(string text)
        {
            if (ScreenNeedUpdate(vertical_aligment.up) == true)
                ScreenUpdate();        
            this.text_buffer.Enqueue(text);
            this.horizontal_text_position_buffer.Enqueue(horizontal_aligment.left); //Writing from left, default setting
            this.last_text_position = vertical_aligment.up; //last position update
            ScreenUpdate();
        }

        /// <summary>
        /// This method allows you to write line of text with aligment to left, center or right.
        /// </summary>
        /// <param name="text">Twxt to write to the console.</param>
        /// <param name="horizontal_text_position">Text aligment.</param>
        public void WriteLine(string text, horizontal_aligment horizontal_text_position)
        {
            if (ScreenNeedUpdate(vertical_aligment.up) == true)
                ScreenUpdate();
            this.text_buffer.Enqueue(text);
            this.horizontal_text_position_buffer.Enqueue(horizontal_text_position);
            this.last_text_position = vertical_aligment.up;
            ScreenUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="text_position_horizontal"></param>
        /// <param name="text_position_vertical"></param>
        public void WriteLine(string text, horizontal_aligment text_position_horizontal, vertical_aligment text_position_vertical)
        {
            if (ScreenNeedUpdate(text_position_vertical) == true)
                ScreenUpdate();
            this.text_buffer.Enqueue(text);
            this.horizontal_text_position_buffer.Enqueue(text_position_horizontal);
            this.last_text_position = text_position_vertical;
        }

        public void WriteLineAt(string text, int x_position, int y_position)
        {
            Console.SetCursorPosition(x_position, y_position);
            Console.WriteLine(text);
            this.last_line = y_position;
            ScreenUpdate();
        }

        private bool ScreenNeedUpdate(vertical_aligment position)
        {
            if (position.Equals(last_text_position))
                return false;
            else
                return true;
        }

        public void ScreenUpdate()
        {
            
            Debug.WriteLine("Printed line"+text_buffer.Count+" at "+last_text_position);
            int count = 0;
            foreach(string text in text_buffer)
            {   
                WriteAtLine(text,horizontal_text_position_buffer.Dequeue(), last_text_position, this.text_buffer.Count, count);
                count++;
            }
            
            this.text_buffer.Clear();
            this.horizontal_text_position_buffer.Clear();
            this.text_x_position.Clear();
            this.text_y_position.Clear();
        }


    }
}
