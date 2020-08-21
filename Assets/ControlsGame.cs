// GENERATED AUTOMATICALLY FROM 'Assets/ControlsGame.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @ControlsGame : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @ControlsGame()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""ControlsGame"",
    ""maps"": [
        {
            ""name"": ""MainControls"",
            ""id"": ""6cfd5c3e-9207-4a99-9986-58e6670b514b"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""d46ece76-8cdc-4698-ba5b-9da16e873fb4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SprintToggle"",
                    ""type"": ""Button"",
                    ""id"": ""8cca4dcc-dd5b-4d76-93d9-d1f3bc5bfd87"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LockedOrbit"",
                    ""type"": ""Button"",
                    ""id"": ""f2ba122a-bda4-43e0-b71a-87820ce1c8b2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""FreeOrbit"",
                    ""type"": ""Button"",
                    ""id"": ""856fb4d7-8fcd-4943-a18f-132e191521f0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""13f75a28-37a4-4c80-b043-b94b2ef7b6e0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack one"",
                    ""type"": ""Button"",
                    ""id"": ""221d6944-325f-4946-bae1-8d6f72cbfbe7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack two"",
                    ""type"": ""Button"",
                    ""id"": ""fbaf34dd-03df-4a19-87a8-52158fae52e5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RollForward"",
                    ""type"": ""Button"",
                    ""id"": ""c58c516f-ded4-494b-a8af-ce8a96e60397"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MouseDelta"",
                    ""type"": ""Value"",
                    ""id"": ""a6b4919e-76b8-4f97-9add-25608e611425"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RollBackward"",
                    ""type"": ""Button"",
                    ""id"": ""50321ea2-7544-4b5e-8a16-18164f132ba4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EquipmentSwitch"",
                    ""type"": ""Button"",
                    ""id"": ""01b0aee6-1862-4ccb-94d7-e498d446588b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack three"",
                    ""type"": ""Button"",
                    ""id"": ""9756c175-a237-4024-82e5-2cb58fbf30d5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""7fdb902e-2d08-448d-b675-8011506dab7a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToggleQuestPanel"",
                    ""type"": ""Button"",
                    ""id"": ""e58e1645-cc50-42a4-b9c5-d38b92515f32"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToggleInventoryPanel"",
                    ""type"": ""Button"",
                    ""id"": ""f68174d8-035b-4421-ae40-dbbe77ddebec"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToggleMainMenu"",
                    ""type"": ""Button"",
                    ""id"": ""3de67d6c-b545-49db-8bb4-06b2746ac0a7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""36f44395-05e5-4104-98be-f373300dd9b0"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f1f10564-c183-4efb-a9b9-7e676f7cb334"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2e5a004c-947e-4be4-af75-b64a57b6910d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b3f9b71a-b936-48b2-b070-00107cc3aa0f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""76be33fc-a98f-4a54-9f6b-4342c30c1343"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""25a1edb8-adb7-4776-bcc4-52f4aa41c721"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""SprintToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a39726a2-1342-4fe0-9985-edde723021d2"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""LockedOrbit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b07a2ec6-7bca-4b59-be9e-a45d0ef53b8a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""FreeOrbit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c75739e4-b1dc-4a95-b86f-86c8d4914678"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e219ec4b-ffbd-4c32-841f-78076ed3d134"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Attack one"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d598b90-5d4f-4f05-bf39-809c4865a6b7"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Attack two"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""17b35382-f0f9-4810-9304-ec6ebc9bc35e"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""RollForward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""57e3a64e-0b83-49f3-b3f2-00a5c77c3260"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""MouseDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4831e64a-6de3-4105-ab7d-0618c3608c76"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""RollBackward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""59fae8f2-27bc-4354-a128-d75a98c93414"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""EquipmentSwitch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""63756fa8-f7ee-48f6-ba93-32e2f59c2eca"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Attack three"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f884e910-ddbf-471d-a4de-8f9b9027d229"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71edb941-37b7-4724-838d-c70abecc6021"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleQuestPanel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b3b28dfa-1fa4-4aca-82db-19893fcc8467"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleInventoryPanel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""31bef7c0-df2a-4759-8bd2-d8c317946173"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleMainMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and mouse"",
            ""bindingGroup"": ""Keyboard and mouse"",
            ""devices"": []
        }
    ]
}");
        // MainControls
        m_MainControls = asset.FindActionMap("MainControls", throwIfNotFound: true);
        m_MainControls_Movement = m_MainControls.FindAction("Movement", throwIfNotFound: true);
        m_MainControls_SprintToggle = m_MainControls.FindAction("SprintToggle", throwIfNotFound: true);
        m_MainControls_LockedOrbit = m_MainControls.FindAction("LockedOrbit", throwIfNotFound: true);
        m_MainControls_FreeOrbit = m_MainControls.FindAction("FreeOrbit", throwIfNotFound: true);
        m_MainControls_Jump = m_MainControls.FindAction("Jump", throwIfNotFound: true);
        m_MainControls_Attackone = m_MainControls.FindAction("Attack one", throwIfNotFound: true);
        m_MainControls_Attacktwo = m_MainControls.FindAction("Attack two", throwIfNotFound: true);
        m_MainControls_RollForward = m_MainControls.FindAction("RollForward", throwIfNotFound: true);
        m_MainControls_MouseDelta = m_MainControls.FindAction("MouseDelta", throwIfNotFound: true);
        m_MainControls_RollBackward = m_MainControls.FindAction("RollBackward", throwIfNotFound: true);
        m_MainControls_EquipmentSwitch = m_MainControls.FindAction("EquipmentSwitch", throwIfNotFound: true);
        m_MainControls_Attackthree = m_MainControls.FindAction("Attack three", throwIfNotFound: true);
        m_MainControls_Interact = m_MainControls.FindAction("Interact", throwIfNotFound: true);
        m_MainControls_ToggleQuestPanel = m_MainControls.FindAction("ToggleQuestPanel", throwIfNotFound: true);
        m_MainControls_ToggleInventoryPanel = m_MainControls.FindAction("ToggleInventoryPanel", throwIfNotFound: true);
        m_MainControls_ToggleMainMenu = m_MainControls.FindAction("ToggleMainMenu", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // MainControls
    private readonly InputActionMap m_MainControls;
    private IMainControlsActions m_MainControlsActionsCallbackInterface;
    private readonly InputAction m_MainControls_Movement;
    private readonly InputAction m_MainControls_SprintToggle;
    private readonly InputAction m_MainControls_LockedOrbit;
    private readonly InputAction m_MainControls_FreeOrbit;
    private readonly InputAction m_MainControls_Jump;
    private readonly InputAction m_MainControls_Attackone;
    private readonly InputAction m_MainControls_Attacktwo;
    private readonly InputAction m_MainControls_RollForward;
    private readonly InputAction m_MainControls_MouseDelta;
    private readonly InputAction m_MainControls_RollBackward;
    private readonly InputAction m_MainControls_EquipmentSwitch;
    private readonly InputAction m_MainControls_Attackthree;
    private readonly InputAction m_MainControls_Interact;
    private readonly InputAction m_MainControls_ToggleQuestPanel;
    private readonly InputAction m_MainControls_ToggleInventoryPanel;
    private readonly InputAction m_MainControls_ToggleMainMenu;
    public struct MainControlsActions
    {
        private @ControlsGame m_Wrapper;
        public MainControlsActions(@ControlsGame wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_MainControls_Movement;
        public InputAction @SprintToggle => m_Wrapper.m_MainControls_SprintToggle;
        public InputAction @LockedOrbit => m_Wrapper.m_MainControls_LockedOrbit;
        public InputAction @FreeOrbit => m_Wrapper.m_MainControls_FreeOrbit;
        public InputAction @Jump => m_Wrapper.m_MainControls_Jump;
        public InputAction @Attackone => m_Wrapper.m_MainControls_Attackone;
        public InputAction @Attacktwo => m_Wrapper.m_MainControls_Attacktwo;
        public InputAction @RollForward => m_Wrapper.m_MainControls_RollForward;
        public InputAction @MouseDelta => m_Wrapper.m_MainControls_MouseDelta;
        public InputAction @RollBackward => m_Wrapper.m_MainControls_RollBackward;
        public InputAction @EquipmentSwitch => m_Wrapper.m_MainControls_EquipmentSwitch;
        public InputAction @Attackthree => m_Wrapper.m_MainControls_Attackthree;
        public InputAction @Interact => m_Wrapper.m_MainControls_Interact;
        public InputAction @ToggleQuestPanel => m_Wrapper.m_MainControls_ToggleQuestPanel;
        public InputAction @ToggleInventoryPanel => m_Wrapper.m_MainControls_ToggleInventoryPanel;
        public InputAction @ToggleMainMenu => m_Wrapper.m_MainControls_ToggleMainMenu;
        public InputActionMap Get() { return m_Wrapper.m_MainControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MainControlsActions set) { return set.Get(); }
        public void SetCallbacks(IMainControlsActions instance)
        {
            if (m_Wrapper.m_MainControlsActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnMovement;
                @SprintToggle.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnSprintToggle;
                @SprintToggle.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnSprintToggle;
                @SprintToggle.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnSprintToggle;
                @LockedOrbit.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnLockedOrbit;
                @LockedOrbit.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnLockedOrbit;
                @LockedOrbit.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnLockedOrbit;
                @FreeOrbit.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnFreeOrbit;
                @FreeOrbit.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnFreeOrbit;
                @FreeOrbit.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnFreeOrbit;
                @Jump.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnJump;
                @Attackone.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnAttackone;
                @Attackone.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnAttackone;
                @Attackone.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnAttackone;
                @Attacktwo.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnAttacktwo;
                @Attacktwo.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnAttacktwo;
                @Attacktwo.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnAttacktwo;
                @RollForward.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnRollForward;
                @RollForward.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnRollForward;
                @RollForward.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnRollForward;
                @MouseDelta.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnMouseDelta;
                @MouseDelta.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnMouseDelta;
                @MouseDelta.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnMouseDelta;
                @RollBackward.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnRollBackward;
                @RollBackward.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnRollBackward;
                @RollBackward.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnRollBackward;
                @EquipmentSwitch.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnEquipmentSwitch;
                @EquipmentSwitch.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnEquipmentSwitch;
                @EquipmentSwitch.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnEquipmentSwitch;
                @Attackthree.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnAttackthree;
                @Attackthree.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnAttackthree;
                @Attackthree.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnAttackthree;
                @Interact.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnInteract;
                @ToggleQuestPanel.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnToggleQuestPanel;
                @ToggleQuestPanel.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnToggleQuestPanel;
                @ToggleQuestPanel.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnToggleQuestPanel;
                @ToggleInventoryPanel.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnToggleInventoryPanel;
                @ToggleInventoryPanel.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnToggleInventoryPanel;
                @ToggleInventoryPanel.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnToggleInventoryPanel;
                @ToggleMainMenu.started -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnToggleMainMenu;
                @ToggleMainMenu.performed -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnToggleMainMenu;
                @ToggleMainMenu.canceled -= m_Wrapper.m_MainControlsActionsCallbackInterface.OnToggleMainMenu;
            }
            m_Wrapper.m_MainControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @SprintToggle.started += instance.OnSprintToggle;
                @SprintToggle.performed += instance.OnSprintToggle;
                @SprintToggle.canceled += instance.OnSprintToggle;
                @LockedOrbit.started += instance.OnLockedOrbit;
                @LockedOrbit.performed += instance.OnLockedOrbit;
                @LockedOrbit.canceled += instance.OnLockedOrbit;
                @FreeOrbit.started += instance.OnFreeOrbit;
                @FreeOrbit.performed += instance.OnFreeOrbit;
                @FreeOrbit.canceled += instance.OnFreeOrbit;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attackone.started += instance.OnAttackone;
                @Attackone.performed += instance.OnAttackone;
                @Attackone.canceled += instance.OnAttackone;
                @Attacktwo.started += instance.OnAttacktwo;
                @Attacktwo.performed += instance.OnAttacktwo;
                @Attacktwo.canceled += instance.OnAttacktwo;
                @RollForward.started += instance.OnRollForward;
                @RollForward.performed += instance.OnRollForward;
                @RollForward.canceled += instance.OnRollForward;
                @MouseDelta.started += instance.OnMouseDelta;
                @MouseDelta.performed += instance.OnMouseDelta;
                @MouseDelta.canceled += instance.OnMouseDelta;
                @RollBackward.started += instance.OnRollBackward;
                @RollBackward.performed += instance.OnRollBackward;
                @RollBackward.canceled += instance.OnRollBackward;
                @EquipmentSwitch.started += instance.OnEquipmentSwitch;
                @EquipmentSwitch.performed += instance.OnEquipmentSwitch;
                @EquipmentSwitch.canceled += instance.OnEquipmentSwitch;
                @Attackthree.started += instance.OnAttackthree;
                @Attackthree.performed += instance.OnAttackthree;
                @Attackthree.canceled += instance.OnAttackthree;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @ToggleQuestPanel.started += instance.OnToggleQuestPanel;
                @ToggleQuestPanel.performed += instance.OnToggleQuestPanel;
                @ToggleQuestPanel.canceled += instance.OnToggleQuestPanel;
                @ToggleInventoryPanel.started += instance.OnToggleInventoryPanel;
                @ToggleInventoryPanel.performed += instance.OnToggleInventoryPanel;
                @ToggleInventoryPanel.canceled += instance.OnToggleInventoryPanel;
                @ToggleMainMenu.started += instance.OnToggleMainMenu;
                @ToggleMainMenu.performed += instance.OnToggleMainMenu;
                @ToggleMainMenu.canceled += instance.OnToggleMainMenu;
            }
        }
    }
    public MainControlsActions @MainControls => new MainControlsActions(this);
    private int m_KeyboardandmouseSchemeIndex = -1;
    public InputControlScheme KeyboardandmouseScheme
    {
        get
        {
            if (m_KeyboardandmouseSchemeIndex == -1) m_KeyboardandmouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and mouse");
            return asset.controlSchemes[m_KeyboardandmouseSchemeIndex];
        }
    }
    public interface IMainControlsActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnSprintToggle(InputAction.CallbackContext context);
        void OnLockedOrbit(InputAction.CallbackContext context);
        void OnFreeOrbit(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttackone(InputAction.CallbackContext context);
        void OnAttacktwo(InputAction.CallbackContext context);
        void OnRollForward(InputAction.CallbackContext context);
        void OnMouseDelta(InputAction.CallbackContext context);
        void OnRollBackward(InputAction.CallbackContext context);
        void OnEquipmentSwitch(InputAction.CallbackContext context);
        void OnAttackthree(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnToggleQuestPanel(InputAction.CallbackContext context);
        void OnToggleInventoryPanel(InputAction.CallbackContext context);
        void OnToggleMainMenu(InputAction.CallbackContext context);
    }
}
