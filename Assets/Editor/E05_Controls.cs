using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;

namespace UIElementsExamples
{
    public class E05_Controls : EditorWindow
    {
        [MenuItem("UIElementsExamples/05_Controls")]
        public static void ShowExample()
        {
            E05_Controls window = GetWindow<E05_Controls>();
            window.minSize = new Vector2(450, 200);
            window.titleContent = new GUIContent("Example 5");
        }

        [SerializeField]
        List<string> m_Tasks;

        DoubleField m_TextField;
        ScrollView m_TasksContainer;
        Button m_Button;

        public void AddTaskOnReturnKey(KeyDownEvent e)
        {
            if (e.character == '\n')
            {
                m_TextField.UpdateValueFromText();
                AddTask();
                e.PreventDefault();
            }
        }

        public void OnEnable()
        {
            var root = this.GetRootVisualContainer();
            root.AddStyleSheetPath("todolist");

            Label l = new Label("Task name");
            root.Add(l);
            m_TextField = new DoubleField() { name = "input" };
            var dragger = new FieldMouseDragger<double>(m_TextField);
            dragger.SetDragZone(l);
            root.Add(m_TextField);
            m_TextField.RegisterCallback<KeyDownEvent>(AddTaskOnReturnKey);

            m_Button = new Button(AddTask) { text = "Save task" };
            root.Add(m_Button);

            m_TextField.RegisterCallback<InputEvent>(OnInput);
            m_TextField.OnChange(OnChange);

            m_TasksContainer = new ScrollView();
            m_TasksContainer.showHorizontal = false;
            root.Add(m_TasksContainer);

            if (m_Tasks != null)
            {
                foreach(string task in m_Tasks)
                {
                    m_TasksContainer.contentContainer.Add(CreateTask(task));
                }
            }
        }
        void OnInput(InputEvent evt)
        {
            m_Button.text += ".";
        }

        void OnChange(ChangeEvent<double> evt)
        {
            if (evt.newValue > evt.previousValue)
            {
                m_Button.text = "Save bigger task";
            }
            else if (evt.newValue < evt.previousValue)
            {
                m_Button.text = "Save smaller task";
            }
            else
            {
                m_Button.text = "Save task";
            }
        }

        public void DeleteTask(KeyDownEvent e, VisualElement task)
        {
            if (e.keyCode == KeyCode.Delete)
            {
                if (task != null)
                {
                    task.parent.Remove(task);
                }
            }
        }

        public VisualElement CreateTask(string name)
        {
            var task = new VisualElement();
            task.focusIndex = 0;
            task.name = name;
            task.AddToClassList("task");

            task.RegisterCallback<KeyDownEvent, VisualElement>(DeleteTask, task);

            var taskName = new Toggle(() => {}) { text = name, name = "checkbox" };
            task.Add(taskName);

            var taskDelete = new Button(() => task.parent.Remove(task)) { name = "delete", text = "Delete" };
            task.Add(taskDelete);

            return task;
        }

        public void OnDisable()
        {
            m_Tasks = new List<string>();
            foreach(VisualElement task in m_TasksContainer)
            {
                m_Tasks.Add(task.name);
            }
        }

        void AddTask()
        {
            if (!string.IsNullOrEmpty(m_TextField.text))
            {
                m_TasksContainer.contentContainer.Add(CreateTask(m_TextField.text));
                m_TextField.text = "";

                // Give focus back to text field.
                m_TextField.Focus();
            }
        }

        void OnClick()
        {
            Debug.Log("Hello!");
        }
    }
}